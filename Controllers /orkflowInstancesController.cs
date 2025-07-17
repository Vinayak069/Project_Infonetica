using Microsoft.AspNetCore.Mvc;
using WorkflowEngine.Exceptions;
using WorkflowEngine.Models.DTOs;
using WorkflowEngine.Services;

namespace WorkflowEngine.Controllers;

/// <summary>
/// API controller for workflow instance operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class WorkflowInstancesController : ControllerBase
{
    private readonly IWorkflowEngine _workflowEngine;

    public WorkflowInstancesController(IWorkflowEngine workflowEngine)
    {
        _workflowEngine = workflowEngine;
    }

    /// <summary>
    /// Start a new workflow instance
    /// </summary>
    /// <param name="dto">The workflow instance data</param>
    /// <returns>The created workflow instance</returns>
    [HttpPost]
    public async Task<IActionResult> StartWorkflowInstance([FromBody] CreateWorkflowInstanceDto dto)
    {
        try
        {
            var instance = await _workflowEngine.StartWorkflowInstanceAsync(dto);
            var response = MapToDto(instance);
            return CreatedAtAction(nameof(GetWorkflowInstance), new { id = instance.Id }, response);
        }
        catch (WorkflowDefinitionNotFoundException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidWorkflowDefinitionException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An internal error occurred", details = ex.Message });
        }
    }

    /// <summary>
    /// Get a workflow instance by ID
    /// </summary>
    /// <param name="id">The workflow instance ID</param>
    /// <returns>The workflow instance</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetWorkflowInstance(string id)
    {
        try
        {
            var instance = await _workflowEngine.GetWorkflowInstanceAsync(id);
            var response = MapToDto(instance);
            return Ok(response);
        }
        catch (WorkflowInstanceNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An internal error occurred", details = ex.Message });
        }
    }

    /// <summary>
    /// Get all workflow instances
    /// </summary>
    /// <returns>List of all workflow instances</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllWorkflowInstances()
    {
        try
        {
            var instances = await _workflowEngine.GetAllWorkflowInstancesAsync();
            var response = instances.Select(MapToDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An internal error occurred", details = ex.Message });
        }
    }

    /// <summary>
    /// Get workflow instances by definition ID
    /// </summary>
    /// <param name="definitionId">The workflow definition ID</param>
    /// <returns>List of workflow instances for the definition</returns>
    [HttpGet("by-definition/{definitionId}")]
    public async Task<IActionResult> GetWorkflowInstancesByDefinition(string definitionId)
    {
        try
        {
            var instances = await _workflowEngine.GetWorkflowInstancesByDefinitionAsync(definitionId);
            var response = instances.Select(MapToDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An internal error occurred", details = ex.Message });
        }
    }

    /// <summary>
    /// Execute an action on a workflow instance
    /// </summary>
    /// <param name="id">The workflow instance ID</param>
    /// <param name="dto">The action execution data</param>
    /// <returns>The updated workflow instance</returns>
    [HttpPost("{id}/execute")]
    public async Task<IActionResult> ExecuteAction(string id, [FromBody] ExecuteActionDto dto)
    {
        try
        {
            var instance = await _workflowEngine.ExecuteActionAsync(id, dto);
            var response = MapToDto(instance);
            return Ok(response);
        }
        catch (WorkflowInstanceNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidActionException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (FinalStateException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An internal error occurred", details = ex.Message });
        }
    }

    /// <summary>
    /// Get available actions for a workflow instance
    /// </summary>
    /// <param name="id">The workflow instance ID</param>
    /// <returns>List of available actions</returns>
    [HttpGet("{id}/available-actions")]
    public async Task<IActionResult> GetAvailableActions(string id)
    {
        try
        {
            var actions = await _workflowEngine.GetAvailableActionsAsync(id);
            return Ok(actions);
        }
        catch (WorkflowInstanceNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An internal error occurred", details = ex.Message });
        }
    }

    private static WorkflowInstanceDto MapToDto(WorkflowEngine.Models.WorkflowInstance instance)
    {
        return new WorkflowInstanceDto
        {
            Id = instance.Id,
            DefinitionId = instance.DefinitionId,
            CurrentState = instance.CurrentState,
            History = instance.History.Select(h => new HistoryEntryDto
            {
                ActionId = h.ActionId,
                FromState = h.FromState,
                ToState = h.ToState,
                Timestamp = h.Timestamp,
                Notes = h.Notes
            }).ToList(),
            CreatedAt = instance.CreatedAt,
            UpdatedAt = instance.UpdatedAt,
            Metadata = instance.Metadata
        };
    }
}
