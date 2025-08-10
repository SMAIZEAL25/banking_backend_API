using BankingApp.Application.DTOs;
using BankingApp.Domain.Enums;
using FluentValidation;
using System;
using System.Text.RegularExpressions;

namespace BankingApp.Application.Validators
{
    public class AccountOpeningValidator : AbstractValidator<UserDTO>
    {
        public AccountOpeningValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);

            RuleFor(x => x.Email)
                .NotEmpty().EmailAddress()
                .WithMessage("Invalid email format");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?234[0-9]{10}$")
                .WithMessage("Phone number must be in Nigerian format e.g. +234xxxxxxxxxx");

            RuleFor(x => x.StateOfOrigin)
                .NotEmpty().Must(BeAValidNigerianState)
                .WithMessage("Invalid Nigerian state");

            RuleFor(x => x.DateOfBirth)
                .Must(BeAtLeast18YearsOld)
                .WithMessage("User must be at least 18 years old");
        }

        private bool BeAtLeast18YearsOld(DateTime dob)
        {
            return dob <= DateTime.Today.AddYears(-18);
        }

        private bool BeAValidNigerianState(string state)
        {
            var states = new[]
            {
                "Lagos", "Abuja", "Kano", "Enugu", "Kaduna", "Oyo", "Rivers"
                // Add all Nigerian states...
            };
            return states.Contains(state, StringComparer.OrdinalIgnoreCase);
        }
    }
}
