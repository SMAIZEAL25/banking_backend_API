using BankingApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Application.DTOs
{
    public class UserDTO
    {
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

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

    }
}
