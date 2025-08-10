using BankingApp.Domain.Enums;

namespace BankingApp.Application.DTOs
{
    public class DepositResponseDto
    {
        public string AccountNumber { get; set; }
        public CurrencyTypes Currency { get; set; }
        public string Message { get; set; }
        public decimal NewBalance { get; set; }
    }
}