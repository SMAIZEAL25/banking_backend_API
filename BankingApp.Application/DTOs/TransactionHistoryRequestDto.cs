using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Application.DTOs
{
    public class TransactionHistoryDto
    {
        public int TransactionId { get; set; }
        
        public string TransactionDate { get; set; }
        public string TransactionTime { get; set; }
        public string TransactionType { get; set; }
        public string Description { get; set; }
        public string BeneficiaryName { get; set; }
        public string BeneficiaryAccountNumber { get; set; }
        public string AccountName { get; set; }
        public string BeneficiaryBankName { get; set; }
        public string TransactionStatus { get; set; }
    }
}
