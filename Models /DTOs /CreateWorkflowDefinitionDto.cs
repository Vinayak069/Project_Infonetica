namespace WorkflowEngine.Models.DTOs;

/// <summary>
/// DTO for creating a new workflow definition
/// </summary>
public class CreateWorkflowDefinitionDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required List<StateDto> States { get; set; } = new();
    public required List<ActionDto> Actions { get; set; } = new();
}

/// <summary>
/// DTO for state information
/// </summary>
public class StateDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public bool IsInitial { get; set; } = false;
    public bool IsFinal { get; set; } = false;
    public bool Enabled { get; set; } = true;
    public string? Description { get; set; }
}

/// <summary>
/// DTO for action information
/// </summary>
public class ActionDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public bool Enabled { get; set; } = true;
    public required List<string> FromStates { get; set; } = new();
    public required string ToState { get; set; }
    public string? Description { get; set; }
}
