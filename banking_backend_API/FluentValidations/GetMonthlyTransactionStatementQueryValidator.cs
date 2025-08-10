using FluentValidation;
using BankingApp.Application.Queries;

namespace BankingApp.Application.Validators
{
    public class GetMonthlyTransactionStatementQueryValidator
        : AbstractValidator<GetMonthlyTransactionStatementQuery>
    {
        public GetMonthlyTransactionStatementQueryValidator()
        {
            RuleFor(x => x.AccountNumber)
                .NotEmpty().WithMessage("Account number is required.")
                .Length(10).WithMessage("Account number must be exactly 10 digits.");

            RuleFor(x => x.NumberOfMonths)
                .GreaterThan(0).WithMessage("Number of months must be greater than zero.")
                .LessThanOrEqualTo(12).WithMessage("You can only request up to 12 months.");
        }
    }
}
