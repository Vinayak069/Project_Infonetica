namespace WorkflowEngine.Models;

/// <summary>
/// Represents a state in a workflow with required attributes
/// </summary>
public class State
{
    /// <summary>
    /// Unique identifier for the state
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Human-readable name for the state
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Indicates if this is the initial state of the workflow
    /// </summary>
    public bool IsInitial { get; set; } = false;

    /// <summary>
    /// Indicates if this is a final state of the workflow
    /// </summary>
    public bool IsFinal { get; set; } = false;

    /// <summary>
    /// Indicates if the state is currently enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Optional description of the state
    /// </summary>
    public string? Description { get; set; }
}
