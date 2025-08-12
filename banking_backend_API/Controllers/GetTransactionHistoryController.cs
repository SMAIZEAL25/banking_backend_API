using BankingApp.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BankingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IMediator mediator, ILogger<AccountsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{accountNumber}/transactions")]
        public async Task<IActionResult> GetTransactionHistory(string accountNumber)
        {
            try
            {
                var result = await _mediator.Send(new GetAccountTransactionHistoryQuery(accountNumber));
                return result.IsSuccess ? Ok(result) : NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transaction history for {AccountNumber}", accountNumber);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{accountNumber}/monthly-statement/{months}")]
        public async Task<IActionResult> GetMonthlyStatement(string accountNumber, int months)
        {
            try
            {
                var result = await _mediator.Send(new GetMonthlyTransactionStatementQuery(accountNumber, months));
                return result.IsSuccess ? Ok(result) : NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving monthly statement for {AccountNumber}", accountNumber);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
