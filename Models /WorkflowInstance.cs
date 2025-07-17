namespace WorkflowEngine.Models;

/// <summary>
/// Represents a running instance of a workflow with current state and history
/// </summary>
public class WorkflowInstance
{
    /// <summary>
    /// Unique identifier for the workflow instance
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Reference to the workflow definition this instance follows
    /// </summary>
    public required string DefinitionId { get; set; }

    /// <summary>
    /// Current state ID of the instance
    /// </summary>
    public required string CurrentState { get; set; }

    /// <summary>
    /// Basic history of actions performed (action + timestamp)
    /// </summary>
    public List<HistoryEntry> History { get; set; } = new();

    /// <summary>
    /// Timestamp when the instance was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when the instance was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Optional metadata for the instance
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Represents a history entry in the workflow instance
/// </summary>
public class HistoryEntry
{
    /// <summary>
    /// The action that was performed
    /// </summary>
    public required string ActionId { get; set; }

    /// <summary>
    /// The state before the action
    /// </summary>
    public required string FromState { get; set; }

    /// <summary>
    /// The state after the action
    /// </summary>
    public required string ToState { get; set; }

    /// <summary>
    /// When the action was performed
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Optional additional context
    /// </summary>
    public string? Notes { get; set; }
}
