using BankingApp.Application.DTOs;
using BankingApp.Application.Features.Transactions.Withdrawal;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace banking_backend_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WithdrawController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<WithdrawController> _logger;

        public WithdrawController(IMediator mediator, ILogger<WithdrawController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("withdraw")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawRequestDto request)
        {
            _logger.LogInformation("Withdraw request initiated for Account: {AccountNumber}", request.AccountNumber);

            var validator = new WithdrawRequestValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Withdraw request validation failed for Account: {AccountNumber}. Errors: {Errors}",
                    request.AccountNumber,
                    string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                return BadRequest(validationResult.Errors);
            }

            var result = await _mediator.Send(new WithdrawCommand(request));

            if (!result.Success)
            {
                _logger.LogWarning("Withdraw request failed for Account: {AccountNumber}. Reason: {Reason}",
                    request.AccountNumber,
                    result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Withdraw request successful for Account: {AccountNumber}", request.AccountNumber);
            return Ok(result);
        }
    }
}
