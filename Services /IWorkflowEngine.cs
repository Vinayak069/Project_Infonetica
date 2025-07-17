using WorkflowEngine.Models;
using WorkflowEngine.Models.DTOs;

namespace WorkflowEngine.Services;

/// <summary>
/// Interface for the workflow engine providing all workflow operations
/// </summary>
public interface IWorkflowEngine
{
    // Workflow Definition Operations
    Task<WorkflowDefinition> CreateWorkflowDefinitionAsync(CreateWorkflowDefinitionDto dto);
    Task<WorkflowDefinition> GetWorkflowDefinitionAsync(string definitionId);
    Task<IEnumerable<WorkflowDefinition>> GetAllWorkflowDefinitionsAsync();

    // Workflow Instance Operations
    Task<WorkflowInstance> StartWorkflowInstanceAsync(CreateWorkflowInstanceDto dto);
    Task<WorkflowInstance> GetWorkflowInstanceAsync(string instanceId);
    Task<IEnumerable<WorkflowInstance>> GetAllWorkflowInstancesAsync();
    Task<IEnumerable<WorkflowInstance>> GetWorkflowInstancesByDefinitionAsync(string definitionId);

    // Action Execution
    Task<WorkflowInstance> ExecuteActionAsync(string instanceId, ExecuteActionDto dto);
    Task<IEnumerable<Models.Action>> GetAvailableActionsAsync(string instanceId);

    // Validation
    Task<bool> ValidateWorkflowDefinitionAsync(WorkflowDefinition definition);
}
