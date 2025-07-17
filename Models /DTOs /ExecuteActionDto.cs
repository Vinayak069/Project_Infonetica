namespace WorkflowEngine.Models.DTOs;

/// <summary>
/// DTO for executing an action on a workflow instance
/// </summary>
public class ExecuteActionDto
{
    public required string ActionId { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for creating a new workflow instance
/// </summary>
public class CreateWorkflowInstanceDto
{
    public required string DefinitionId { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// DTO for workflow instance response
/// </summary>
public class WorkflowInstanceDto
{
    public required string Id { get; set; }
    public required string DefinitionId { get; set; }
    public required string CurrentState { get; set; }
    public List<HistoryEntryDto> History { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// DTO for history entry
/// </summary>
public class HistoryEntryDto
{
    public required string ActionId { get; set; }
    public required string FromState { get; set; }
    public required string ToState { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Notes { get; set; }
}
