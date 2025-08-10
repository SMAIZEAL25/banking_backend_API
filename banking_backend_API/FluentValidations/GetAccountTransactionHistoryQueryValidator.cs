using FluentValidation;
using BankingApp.Application.Queries;

namespace BankingApp.Application.Validators
{
    public class GetAccountTransactionHistoryQueryValidator
        : AbstractValidator<GetAccountTransactionHistoryQuery>
    {
        public GetAccountTransactionHistoryQueryValidator()
        {
            RuleFor(x => x.AccountNumber)
                .NotEmpty().WithMessage("Account number is required.")
                .Length(10).WithMessage("Account number must be exactly 10 digits.");
        }
    }
}
