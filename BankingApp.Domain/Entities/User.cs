using BankingApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BankingApp.Domain.Entities
{
    public class User : IValidatableObject
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string FullName => $"{FirstName} {MiddleName} {LastName}";

        [Required]
        public DateTime DateOfBirth { get; set; }

        // becomes read only after the user is created
        //public int Age
        //{
        //    get
        //    {
        //        if (DateOfBirth == default)
        //            return 0;

        //        var today = DateTime.Today;
        //        var age = today.Year - DateOfBirth.Year;
        //        if (DateOfBirth.Date > today.AddYears(-age)) age--;
        //        return age;
        //    }
        //}


        [Required]
        public long BankVerificationNumber { get; set; }

        [Required]
        public NigerianStates StateOfOrigin { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^\+234\d{10}$", ErrorMessage = "Phone number must be in international format starting with +234")]
        public string PhoneNumber { get; set; }

        public string Occupation { get; set; }

        public MaritalStatus MaritalStatus { get; set; }

        public string Country { get; set; }

        public string LocalGovernmentOrigin { get; set; }

        public ICollection<Account> Accounts { get; set; } = new List<Account>();

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Email Format already been handled by [EmailAddress],
            if (!Regex.IsMatch(Email ?? "", @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                yield return new ValidationResult("Email format is invalid", new[] { nameof(Email) });
            }

            // Validate Date of Birth is in the past
            if (DateOfBirth > DateTime.Today)
            {
                yield return new ValidationResult("Date of birth cannot be in the future.", new[] { nameof(DateOfBirth) });
            }

            // Calculate age
            int age = DateTime.Today.Year - DateOfBirth.Year;
            if (DateOfBirth > DateTime.Today.AddYears(-age)) age--; // adjust if birthday hasn't occurred this year

            if (age < 18)
            {
                yield return new ValidationResult("Applicant must be at least 18 years old.", new[] { nameof(DateOfBirth) });
            }

            // Nigerian phone number following international format using +234
            if (!Regex.IsMatch(PhoneNumber ?? "", @"^\+234\d{10}$"))
            {
                yield return new ValidationResult("Phone number must follow the format +234XXXXXXXXXX", new[] { nameof(PhoneNumber) });
            }

            // Here am maually Checking Requried feilds in case of null bypass 
            var requiredFields = new Dictionary<string, string>
        {
            { nameof(Address), Address },
            { nameof(City), City },
            { nameof(Country), Country },
            { nameof(Occupation), Occupation },
            { nameof(LocalGovernmentOrigin), LocalGovernmentOrigin }
        };

            foreach (var field in requiredFields)
            {
                if (string.IsNullOrWhiteSpace(field.Value))
                {
                    yield return new ValidationResult($"{field.Key} cannot be null or empty", new[] { field.Key });
                }
            }
        }
    }

}
