

using BankingApp.Application.DTOs;

namespace banking_backend_API_By_Solomon_Chika_Samuel.Response
{

    public class AccountOpeningResponse
    {
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public string Message { get; set; }
        public AccountDTO Account { get; set; }
    }

}
