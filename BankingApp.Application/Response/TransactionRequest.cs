using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Application.Response
{
    public class TransactionRequest
    {
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string BeneficiaryName { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string Currency { get; set; } = "NGN";
    }
}
