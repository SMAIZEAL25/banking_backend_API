using Newtonsoft.Json;

namespace BankingApp.Infrastruture.Integration.Callback
{
    public class VerificationData
    {
        [JsonProperty("status")]
        public string Status { get; set; } // "success", "failed", "abandoned"

        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }
}
