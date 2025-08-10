using BankingApp.Application.Commands;
using BankingApp.Application.DTOs;
using BankingApp.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly OpenAccountCommand _openAccountCommand;

    public AccountController(OpenAccountCommand openAccountCommand)
    {
        _openAccountCommand = openAccountCommand;
    }

    [HttpPost("open")]
    public async Task<IActionResult> OpenAccount([FromBody] UserDTO userDto, [FromQuery] AccountType accountType)
    {
        try
        {
            var result = await _openAccountCommand.ExecuteAsync(userDto, accountType);
            return StatusCode(result.StatusCode, result);
        }
        catch (Exception ex)
        {
            // Log exception and return generic error
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }
}
