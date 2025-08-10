using BankingApp.Domain.Enums;
using System;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Application.DTOs
{
    public class AccountDetailsResponseDto
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public decimal CurrentBalance { get; set; }
        public CurrencyTypes Currency { get; set; }
        public AccountStatus AccountStatus { get; set; }
        public DateTime DateOpened { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }
}

