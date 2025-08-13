using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepositController : ControllerBase
{
    private readonly IMediator _mediator;

    public DepositController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("deposit")]
    [Authorize(Roles = "Writer")]
    public async Task<IActionResult> Deposit([FromBody] DepositCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return StatusCode(result.StatusCode, result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { Errors = ex.Errors.Select(e => e.ErrorMessage) });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An unexpected error occurred", Detail = ex.Message });
        }
    }
}