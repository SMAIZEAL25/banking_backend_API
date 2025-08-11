using BankingApp.Application.DTOs;
using BankingApp.Application.Interfaces;
using BankingApp.Application.Interfaces; 
using BankingApp.Domain.Entities;
using BankingApp.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;


namespace BankingApp.Application.Features.Transactions.Withdrawal
{
    public record WithdrawCommand(WithdrawRequestDto Request) : IRequest<WithdrawResponseDto>;

    public class WithdrawCommandHandler : IRequestHandler<WithdrawCommand, WithdrawResponseDto>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WithdrawCommandHandler> _logger;

        public WithdrawCommandHandler(
            IAccountRepository accountRepository,
            IUnitOfWork unitOfWork,
            ILogger<WithdrawCommandHandler> logger)
        {
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<WithdrawResponseDto> Handle(WithdrawCommand request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByAccountNumberAsync(request.Request.AccountNumber);
            if (account == null)
            {
                return new WithdrawResponseDto
                {
                    Success = false,
                    Message = "Account not found."
                };
            }

            if (account.Balance < request.Request.Amount)
            {
                return new WithdrawResponseDto
                {
                    Success = false,
                    Message = "Insufficient funds."
                };
            }

            // Deduct the amount
            account.Balance -= request.Request.Amount;

            // Create transaction
            var transaction = new Transaction
            {
                AccountId = account.Id,
                Amount = request.Request.Amount,
                TransactionType = TransactionType.Debit,
                CurrencyTypes = request.Request.CurrencyTypes,
                TransactionDate = DateTime.UtcNow
            };

            await _accountRepository.UpdateAsync(account);
            await _unitOfWork.TransactionRepository.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Withdrawal successful for {AccountNumber}, Amount: {Amount} {Currency}",
                account.AccountNumber, request.Request.Amount, request.Request.CurrencyTypes);

            return new WithdrawResponseDto
            {
                Success = true,
                Message = "Withdrawal successful.",
                UpdatedBalance = account.Balance
            };
        }
    }
}
