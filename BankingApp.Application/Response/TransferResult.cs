using BankingApp.Domain.Entities;

namespace BankingApp.Infrastruture.Services
{
    public class TransferResult
    {
        public bool IsSuccesTransfer { get; set; }
        public Account SourceAccount { get; set; }
        public Account  BeneficaryAccount { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
        public string AuthorizationUrl { get; set; }
    }

}
