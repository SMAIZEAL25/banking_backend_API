using BankingApp.Application.DTOs;
using BankingApp.Domain.Enums;
using FluentValidation;

public class TransferRequestValidator : AbstractValidator<TransferRequestDto>
{
    public TransferRequestValidator()
    {
        // For string account numbers
        RuleFor(x => x.AccountNumber)
            .NotEmpty().WithMessage("Source account number is required.")
            .Length(10).WithMessage("Source account number must be 10 digits.");

        // For string beneficiary account numbers
        RuleFor(x => x.BeneficiaryAccountNumber)
            .NotEmpty().WithMessage("Beneficiary account number is required.")
            .Length(10).WithMessage("Beneficiary account number must be 10 digits.");

        // For numeric amount
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Transfer amount must be greater than zero.");

        // For enum validation
        RuleFor(x => x.Currency)
            .IsInEnum().WithMessage("Invalid currency type.");

        // For string bank name
        RuleFor(x => x.BeneficiaryBank)
            .NotEmpty().WithMessage("Beneficiary bank is required.");
    }
}