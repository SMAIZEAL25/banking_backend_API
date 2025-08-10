using BankingApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Application.DTOs
{
    public class AccountDTO
    {
        public string AccountNumber { get; set; }

        public AccountType AccountType { get; set; }

        public decimal CurrentBalance { get; set; }

        public CurrencyTypes Currency { get; set; }

        public DateTime OpeningDate { get; set; }

        public string BranchName { get; set; }

        public AccountStatus AccountStatus { get; set; }

        public string AccountOfficer { get; set; }

    }
}
