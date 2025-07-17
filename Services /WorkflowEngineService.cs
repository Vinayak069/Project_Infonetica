using WorkflowEngine.Exceptions;
using WorkflowEngine.Models;
using WorkflowEngine.Models.DTOs;

namespace WorkflowEngine.Services;

/// <summary>
/// Main implementation of the workflow engine
/// </summary>
public class WorkflowEngineService : IWorkflowEngine
{
    private readonly IWorkflowStorage _storage;

    public WorkflowEngineService(IWorkflowStorage storage)
    {
        _storage = storage;
    }

    // Workflow Definition Operations
    public async Task<WorkflowDefinition> CreateWorkflowDefinitionAsync(CreateWorkflowDefinitionDto dto)
    {
        var definition = new WorkflowDefinition
        {
            Id = Guid.NewGuid().ToString(),
            Name = dto.Name,
            Description = dto.Description,
            States = dto.States.Select(s => new State
            {
                Id = s.Id,
                Name = s.Name,
                IsInitial = s.IsInitial,
                IsFinal = s.IsFinal,
                Enabled = s.Enabled,
                Description = s.Description
            }).ToList(),
            Actions = dto.Actions.Select(a => new Models.Action
            {
                Id = a.Id,
                Name = a.Name,
                Enabled = a.Enabled,
                FromStates = a.FromStates,
                ToState = a.ToState,
                Description = a.Description
            }).ToList(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Validate the workflow definition
        if (!await ValidateWorkflowDefinitionAsync(definition))
        {
            throw new InvalidWorkflowDefinitionException("The workflow definition is invalid.");
        }

        await _storage.SaveWorkflowDefinitionAsync(definition);
        return definition;
    }

    public async Task<WorkflowDefinition> GetWorkflowDefinitionAsync(string definitionId)
    {
        var definition = await _storage.GetWorkflowDefinitionAsync(definitionId);
        if (definition == null)
        {
            throw new WorkflowDefinitionNotFoundException(definitionId);
        }
        return definition;
    }

    public async Task<IEnumerable<WorkflowDefinition>> GetAllWorkflowDefinitionsAsync()
    {
        return await _storage.GetAllWorkflowDefinitionsAsync();
    }

    // Workflow Instance Operations
    public async Task<WorkflowInstance> StartWorkflowInstanceAsync(CreateWorkflowInstanceDto dto)
    {
        var definition = await GetWorkflowDefinitionAsync(dto.DefinitionId);
        
        var initialState = definition.States.FirstOrDefault(s => s.IsInitial);
        if (initialState == null)
        {
            throw new InvalidWorkflowDefinitionException($"No initial state found for workflow definition '{dto.DefinitionId}'.");
        }

        var instance = new WorkflowInstance
        {
            Id = Guid.NewGuid().ToString(),
            DefinitionId = dto.DefinitionId,
            CurrentState = initialState.Id,
            Metadata = dto.Metadata,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _storage.SaveWorkflowInstanceAsync(instance);
        return instance;
    }

    public async Task<WorkflowInstance> GetWorkflowInstanceAsync(string instanceId)
    {
        var instance = await _storage.GetWorkflowInstanceAsync(instanceId);
        if (instance == null)
        {
            throw new WorkflowInstanceNotFoundException(instanceId);
        }
        return instance;
    }

    public async Task<IEnumerable<WorkflowInstance>> GetAllWorkflowInstancesAsync()
    {
        return await _storage.GetAllWorkflowInstancesAsync();
    }

    public async Task<IEnumerable<WorkflowInstance>> GetWorkflowInstancesByDefinitionAsync(string definitionId)
    {
        return await _storage.GetWorkflowInstancesByDefinitionAsync(definitionId);
    }

    // Action Execution
    public async Task<WorkflowInstance> ExecuteActionAsync(string instanceId, ExecuteActionDto dto)
    {
        var instance = await GetWorkflowInstanceAsync(instanceId);
        var definition = await GetWorkflowDefinitionAsync(instance.DefinitionId);

        // Get current state
        var currentState = definition.States.FirstOrDefault(s => s.Id == instance.CurrentState);
        if (currentState == null)
        {
            throw new InvalidActionException($"Current state '{instance.CurrentState}' not found in workflow definition.");
        }

        // Check if current state is final
        if (currentState.IsFinal)
        {
            throw new FinalStateException(currentState.Id);
        }

        // Find the action
        var action = definition.Actions.FirstOrDefault(a => a.Id == dto.ActionId);
        if (action == null)
        {
            throw new InvalidActionException($"Action '{dto.ActionId}' not found in workflow definition.");
        }

        // Validate action is enabled
        if (!action.Enabled)
        {
            throw new InvalidActionException($"Action '{dto.ActionId}' is disabled.");
        }

        // Validate action can be executed from current state
        if (!action.FromStates.Contains(instance.CurrentState))
        {
            throw new InvalidActionException($"Action '{dto.ActionId}' cannot be executed from state '{instance.CurrentState}'.");
        }

        // Validate target state exists
        var targetState = definition.States.FirstOrDefault(s => s.Id == action.ToState);
        if (targetState == null)
        {
            throw new InvalidActionException($"Target state '{action.ToState}' not found in workflow definition.");
        }

        // Execute the action
        var historyEntry = new HistoryEntry
        {
            ActionId = action.Id,
            FromState = instance.CurrentState,
            ToState = action.ToState,
            Timestamp = DateTime.UtcNow,
            Notes = dto.Notes
        };

        instance.History.Add(historyEntry);
        instance.CurrentState = action.ToState;
        instance.UpdatedAt = DateTime.UtcNow;

        await _storage.SaveWorkflowInstanceAsync(instance);
        return instance;
    }

    public async Task<IEnumerable<Models.Action>> GetAvailableActionsAsync(string instanceId)
    {
        var instance = await GetWorkflowInstanceAsync(instanceId);
        var definition = await GetWorkflowDefinitionAsync(instance.DefinitionId);

        // Get current state
        var currentState = definition.States.FirstOrDefault(s => s.Id == instance.CurrentState);
        if (currentState == null)
        {
            return Enumerable.Empty<Models.Action>();
        }

        // If current state is final, no actions are available
        if (currentState.IsFinal)
        {
            return Enumerable.Empty<Models.Action>();
        }

        // Return enabled actions that can be executed from current state
        return definition.Actions.Where(a => 
            a.Enabled && 
            a.FromStates.Contains(instance.CurrentState));
    }

    // Validation
    public Task<bool> ValidateWorkflowDefinitionAsync(WorkflowDefinition definition)
    {
        try
        {
            // Check for duplicate state IDs
            var stateIds = definition.States.Select(s => s.Id).ToList();
            if (stateIds.Count != stateIds.Distinct().Count())
            {
                throw new InvalidWorkflowDefinitionException("Duplicate state IDs found.");
            }

            // Check for duplicate action IDs
            var actionIds = definition.Actions.Select(a => a.Id).ToList();
            if (actionIds.Count != actionIds.Distinct().Count())
            {
                throw new InvalidWorkflowDefinitionException("Duplicate action IDs found.");
            }

            // Check for exactly one initial state
            var initialStates = definition.States.Where(s => s.IsInitial).ToList();
            if (initialStates.Count == 0)
            {
                throw new InvalidWorkflowDefinitionException("No initial state found. Exactly one state must be marked as initial.");
            }
            if (initialStates.Count > 1)
            {
                throw new InvalidWorkflowDefinitionException("Multiple initial states found. Exactly one state must be marked as initial.");
            }

            // Validate action references
            foreach (var action in definition.Actions)
            {
                // Check that ToState exists
                if (!definition.States.Any(s => s.Id == action.ToState))
                {
                    throw new InvalidWorkflowDefinitionException($"Action '{action.Id}' references unknown target state '{action.ToState}'.");
                }

                // Check that all FromStates exist
                foreach (var fromState in action.FromStates)
                {
                    if (!definition.States.Any(s => s.Id == fromState))
                    {
                        throw new InvalidWorkflowDefinitionException($"Action '{action.Id}' references unknown source state '{fromState}'.");
                    }
                }

                // Check that action doesn't originate from final states
                foreach (var fromState in action.FromStates)
                {
                    var state = definition.States.First(s => s.Id == fromState);
                    if (state.IsFinal)
                    {
                        throw new InvalidWorkflowDefinitionException($"Action '{action.Id}' cannot originate from final state '{fromState}'.");
                    }
                }
            }

            return Task.FromResult(true);
        }
        catch (InvalidWorkflowDefinitionException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidWorkflowDefinitionException($"Validation failed: {ex.Message}");
        }
    }
}
