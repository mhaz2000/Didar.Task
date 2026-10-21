using Didar.Task.Application.DTOs;
using Didar.Task.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Didar.Task.Api.Controllers;


[ApiController]
[Route("api/experts")]
public sealed class ExpertsController(
    IExpertService expertService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ExpertDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ExpertDto>> Create(
        [FromBody] CreateExpertDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var expert = await expertService.CreateAsync(
                dto,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetAll),
                new { id = expert.Id },
                expert);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<ExpertDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ExpertDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var experts = await expertService.GetAllAsync(cancellationToken);
        return Ok(experts);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ExpertDto>> GetById(
    Guid id,
    CancellationToken cancellationToken)
    {
        var expert = await expertService.GetByIdAsync(
            id,
            cancellationToken);

        return expert is null ? NotFound() : Ok(expert);
    }
}
