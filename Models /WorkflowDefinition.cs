namespace WorkflowEngine.Models;

/// <summary>
/// Represents a workflow definition containing states and actions
/// Must contain exactly one IsInitial == true state
/// </summary>
public class WorkflowDefinition
{
    /// <summary>
    /// Unique identifier for the workflow definition
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Human-readable name for the workflow
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Version of the workflow definition
    /// </summary>
    public string Version { get; set; } = "1.0";

    /// <summary>
    /// Collection of states in this workflow
    /// </summary>
    public required List<State> States { get; set; } = new();

    /// <summary>
    /// Collection of actions in this workflow
    /// </summary>
    public required List<Action> Actions { get; set; } = new();

    /// <summary>
    /// Optional description of the workflow
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Timestamp when the workflow definition was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when the workflow definition was last modified
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
