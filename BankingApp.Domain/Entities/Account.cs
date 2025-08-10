using BankingApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BankingApp.Domain.Entities
{

    public class Account
    {
        public int AccountId { get; set; }
        
        public string AccountName { get; set; }

        public string AccountNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        public AccountType AccountType { get; set; }

        public decimal CurrentBalance { get; set; }

        public CurrencyTypes Currency { get; set; }

        public DateTime OpeningDate { get; set; }

        public string BranchName { get; set; }

        public AccountStatus AccountStatus { get; set; }

        public BankName BeneficiaryBank { get; set; }

        public string AccountOfficer { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }

}
