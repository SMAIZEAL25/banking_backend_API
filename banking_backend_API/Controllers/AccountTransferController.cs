using BankingApp.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TransfersController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransfersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransferRequestDto dto)
    {
        try
        {
            var response = await _mediator.Send(new AccountTransferCommand(dto));
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            // log error
            return StatusCode(500, new { Message = "An error occurred while processing transfer." });
        }
    }
}
