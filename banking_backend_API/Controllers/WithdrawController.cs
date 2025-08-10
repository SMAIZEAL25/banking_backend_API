using BankingApp.Application.DTOs;
using BankingApp.Application.Features.Transactions.Withdrawal;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace banking_backend_API.Controllers
{
    public class WithdrawController
    {
        private readonly IMediator _mediator;

        public WithdrawController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawRequestDto request)
        {
            var validator = new WithdrawRequestValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var result = await _mediator.Send(new WithdrawCommand(request));

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

    }
}
