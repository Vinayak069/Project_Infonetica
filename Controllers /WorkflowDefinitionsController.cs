using Microsoft.AspNetCore.Mvc;
using WorkflowEngine.Exceptions;
using WorkflowEngine.Models.DTOs;
using WorkflowEngine.Services;

namespace WorkflowEngine.Controllers;

/// <summary>
/// API controller for workflow definition operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class WorkflowDefinitionsController : ControllerBase
{
    private readonly IWorkflowEngine _workflowEngine;

    public WorkflowDefinitionsController(IWorkflowEngine workflowEngine)
    {
        _workflowEngine = workflowEngine;
    }

    /// <summary>
    /// Create a new workflow definition
    /// </summary>
    /// <param name="dto">The workflow definition data</param>
    /// <returns>The created workflow definition</returns>
    [HttpPost]
    public async Task<IActionResult> CreateWorkflowDefinition([FromBody] CreateWorkflowDefinitionDto dto)
    {
        try
        {
            var definition = await _workflowEngine.CreateWorkflowDefinitionAsync(dto);
            return CreatedAtAction(nameof(GetWorkflowDefinition), new { id = definition.Id }, definition);
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
    /// Get a workflow definition by ID
    /// </summary>
    /// <param name="id">The workflow definition ID</param>
    /// <returns>The workflow definition</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetWorkflowDefinition(string id)
    {
        try
        {
            var definition = await _workflowEngine.GetWorkflowDefinitionAsync(id);
            return Ok(definition);
        }
        catch (WorkflowDefinitionNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An internal error occurred", details = ex.Message });
        }
    }

    /// <summary>
    /// Get all workflow definitions
    /// </summary>
    /// <returns>List of all workflow definitions</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllWorkflowDefinitions()
    {
        try
        {
            var definitions = await _workflowEngine.GetAllWorkflowDefinitionsAsync();
            return Ok(definitions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An internal error occurred", details = ex.Message });
        }
    }
}
