namespace WorkflowEngine.Exceptions;

/// <summary>
/// Base exception for workflow-related errors
/// </summary>
public class WorkflowException : Exception
{
    public WorkflowException(string message) : base(message) { }
    public WorkflowException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when a workflow definition is invalid
/// </summary>
public class InvalidWorkflowDefinitionException : WorkflowException
{
    public InvalidWorkflowDefinitionException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when a workflow definition is not found
/// </summary>
public class WorkflowDefinitionNotFoundException : WorkflowException
{
    public WorkflowDefinitionNotFoundException(string definitionId) 
        : base($"Workflow definition with ID '{definitionId}' not found.") { }
}

/// <summary>
/// Exception thrown when a workflow instance is not found
/// </summary>
public class WorkflowInstanceNotFoundException : WorkflowException
{
    public WorkflowInstanceNotFoundException(string instanceId) 
        : base($"Workflow instance with ID '{instanceId}' not found.") { }
}

/// <summary>
/// Exception thrown when an invalid action is attempted
/// </summary>
public class InvalidActionException : WorkflowException
{
    public InvalidActionException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when trying to execute an action on a final state
/// </summary>
public class FinalStateException : WorkflowException
{
    public FinalStateException(string stateId) 
        : base($"Cannot execute actions on final state '{stateId}'.") { }
}
