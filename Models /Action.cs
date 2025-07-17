namespace WorkflowEngine.Models;

/// <summary>
/// Represents a transition/action that can move an instance between states
/// </summary>
public class Action
{
    /// <summary>
    /// Unique identifier for the action
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Human-readable name for the action
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Indicates if the action is currently enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Collection of state IDs from which this action can be triggered
    /// </summary>
    public required List<string> FromStates { get; set; } = new();

    /// <summary>
    /// The target state ID where the action leads
    /// Note: One action may originate from multiple states but always ends in one
    /// </summary>
    public required string ToState { get; set; }

    /// <summary>
    /// Optional description of the action
    /// </summary>
    public string? Description { get; set; }
}
