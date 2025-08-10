using BankingApp.Application.DTOs;
using FluentValidation;

namespace BankingApp.Application.Features.Transactions.Withdrawal
{
    public class WithdrawRequestValidator : AbstractValidator<WithdrawRequestDto>
    {
        public WithdrawRequestValidator()
        {
            RuleFor(x => x.AccountNumber)
                .NotEmpty().WithMessage("Account number is required.")
                .Length(10).WithMessage("Account number must be exactly 10 digits.");

            RuleFor(x => x.AccountType)
                .IsInEnum().WithMessage("Invalid account type.");

            RuleFor(x => x.AccountName)
                .NotEmpty().WithMessage("Account name is required.")
                .MaximumLength(100).WithMessage("Account name is too long.");

            RuleFor(x => x.CurrencyTypes)
                .IsInEnum().WithMessage("Invalid currency type.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Withdrawal amount must be greater than zero.")
                .LessThanOrEqualTo(1_000_000).WithMessage("Withdrawal amount exceeds the maximum allowed limit.");
        }
    }
}
