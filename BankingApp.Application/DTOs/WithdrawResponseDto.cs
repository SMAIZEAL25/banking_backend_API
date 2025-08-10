using BankingApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Application.DTOs
{
    public class WithdrawResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public decimal UpdatedBalance { get; set; }
    }
}
