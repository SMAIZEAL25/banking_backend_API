using Newtonsoft.Json;

namespace BankingApp.Infrastruture.Integration.Callback
{
    public class PaystackVerificationResponse
    {
        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public VerificationData Data { get; set; }
    }
}
