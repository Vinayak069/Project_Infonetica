using WorkflowEngine.Models;

namespace WorkflowEngine.Services;

/// <summary>
/// Interface for workflow data persistence
/// </summary>
public interface IWorkflowStorage
{
    // Workflow Definition Storage
    Task SaveWorkflowDefinitionAsync(WorkflowDefinition definition);
    Task<WorkflowDefinition?> GetWorkflowDefinitionAsync(string definitionId);
    Task<IEnumerable<WorkflowDefinition>> GetAllWorkflowDefinitionsAsync();

    // Workflow Instance Storage
    Task SaveWorkflowInstanceAsync(WorkflowInstance instance);
    Task<WorkflowInstance?> GetWorkflowInstanceAsync(string instanceId);
    Task<IEnumerable<WorkflowInstance>> GetAllWorkflowInstancesAsync();
    Task<IEnumerable<WorkflowInstance>> GetWorkflowInstancesByDefinitionAsync(string definitionId);
}
