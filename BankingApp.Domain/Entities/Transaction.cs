using BankingApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BankingApp.Domain.Entities
{
    public class Transaction : IValidatableObject
    {
        public int TransactionId { get; set; }

        [Required]
        public TransactionType TransactionType { get; set; } // Deposit, Withdraw, Transfer

        [Required]
        public decimal Amount { get; set; }

        public CurrencyTypes CurrencyTypes { get; set; }

        public string Description { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public decimal CurrentBalance { get; set; }

        // Transfer-only fields
        public long? BeneficiaryAccountNumber { get; set; } // Changed to nullable long
        public string BeneficiaryName { get; set; }
        public BankName? BeneficiaryBank { get; set; }
        public TransactionStatus TransactionStatus { get; set; }

        // Foreign keys and navigation
        [Required]
        public int AccountId { get; set; }
        public Account Account { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        // Custom validation for Transfer-only fields
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TransactionType == TransactionType.Transfer)
            {
                if (!BeneficiaryAccountNumber.HasValue)
                {
                    yield return new ValidationResult(
                        "Beneficiary Account Number is required for Transfer transactions",
                        new[] { nameof(BeneficiaryAccountNumber) }
                    );
                }

                if (string.IsNullOrWhiteSpace(BeneficiaryName))
                {
                    yield return new ValidationResult(
                        "Beneficiary Name is required for Transfer transactions",
                        new[] { nameof(BeneficiaryName) }
                    );
                }

                if (BeneficiaryBank == null)
                {
                    yield return new ValidationResult(
                        "Beneficiary Bank is required for Transfer transactions",
                        new[] { nameof(BeneficiaryBank) }
                    );
                }
            }
        }
    }
}
