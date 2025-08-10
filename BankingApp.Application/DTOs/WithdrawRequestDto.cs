using BankingApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Application.DTOs
{
    public class WithdrawRequestDto
    {
        public string AccountNumber { get; set; }
        public AccountType AccountType { get; set; }
        public string AccountName { get; set; }
        public CurrencyTypes CurrencyTypes { get; set; }
        public decimal Amount { get; set; }
    }
}
