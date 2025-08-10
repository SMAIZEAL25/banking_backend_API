using BankingApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Application.DTOs
{
    public class DepositRequestDto
    {
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string BeneficiaryName { get; set; }
        public string BankName { get; set; }
        public CurrencyTypes Currency { get; set; }
    }

   
}
