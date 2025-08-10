using Newtonsoft.Json;

namespace BankingApp.Infrastruture.Integration.Response
{
    public class PaystackTransactionResponse
    {
        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public TransactionData Data { get; set; }
    }

    
}
