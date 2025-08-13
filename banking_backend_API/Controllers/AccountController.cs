using BankingApp.Application.Commands;
using BankingApp.Application.DTOs;
using BankingApp.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly OpenAccountCommand _openAccountCommand;
    private readonly ILogger<AccountController> _logger;

    public AccountController(OpenAccountCommand openAccountCommand, ILogger<AccountController> logger)
    {
        _openAccountCommand = openAccountCommand;
        _logger = logger;
    }

    [HttpPost("open")]
    public async Task<IActionResult> OpenAccount([FromBody] UserDTO userDto, [FromQuery] AccountType accountType)
    {
        _logger.LogInformation(
            "Account opening request received for {FullName} with account type {AccountType}",
            userDto.FirstName, accountType);

        try
        {
            var result = await _openAccountCommand.ExecuteAsync(userDto, accountType);

            if (result.StatusCode >= 400)
            {
                _logger.LogWarning(
                    "Account opening failed for {FullName} with account type {AccountType}. Reason: {Message}",
                    userDto.FirstName, accountType, result.Message);
            }
            else
            {
                _logger.LogInformation(
                    "Account successfully opened for {FullName} with account type {AccountType}",
                    userDto.FirstName, accountType);
            }

            return StatusCode(result.StatusCode, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "An unexpected error occurred while opening account for {FullName} with account type {AccountType}",
                userDto.FirstName, accountType);

            return StatusCode(500, new
            {
                message = "Internal server error",
                details = ex.Message
            });
        }
    }
}
