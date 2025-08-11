using BankingApp.Application.DTOs;
using BankingApp.Domain.Enums;
using FluentValidation;

public class TransferRequestValidator : AbstractValidator<TransferRequestDto>
{
    public TransferRequestValidator()
    {
        RuleFor(x => x.AccountNumber)
            .NotEmpty().WithMessage("Source account number is required.")
            .Length(10).WithMessage("Source account number must be 10 digits.");

        RuleFor(x => x.BeneficiaryAccountNumber)
            .NotEmpty().WithMessage("Beneficiary account number is required.")
            .Length(10).WithMessage("Beneficiary account number must be 10 digits.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Transfer amount must be greater than zero.");

        RuleFor(x => x.Currency)
            .IsInEnum().WithMessage("Invalid currency type.");

        RuleFor(x => x.BeneficiaryBank)
            .NotEmpty().WithMessage("Beneficiary bank is required.");
    }
}
