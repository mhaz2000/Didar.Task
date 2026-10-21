using Didar.Task.Application.DTOs;
using Didar.Task.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Didar.Task.Api.Controllers;


[ApiController]
[Route("api/requests")]
public sealed class RequestsController(
    IAssignmentService assignmentService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(RequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RequestDto>> Create(
        [FromBody] CreateRequestDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await assignmentService.CreateRequestAsync(
                dto,
                cancellationToken);

            // 200 is intentional: repeating the same RequestKey
            // returns the existing request (idempotent behavior).
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await assignmentService.GetRequestAsync(
            id,
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("waiting")]
    [ProducesResponseType(
        typeof(IReadOnlyList<RequestDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RequestDto>>> GetWaiting(
        CancellationToken cancellationToken)
    {
        var result =
            await assignmentService.GetWaitingRequestsAsync(
                cancellationToken);

        return Ok(result);
    }

    [HttpPost("{id:guid}/close")]
    [ProducesResponseType(typeof(RequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RequestDto>> Close(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await assignmentService.CloseRequestAsync(
                id,
                cancellationToken);

            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Request not found." });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}