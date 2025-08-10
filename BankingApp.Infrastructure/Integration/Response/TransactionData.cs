using Newtonsoft.Json;

namespace BankingApp.Infrastruture.Integration.Response
{
    public class TransactionData
    {
        [JsonProperty("authorization_url")]
        public string AuthorizationUrl { get; set; }

        [JsonProperty("access_code")]
        public string AccessCode { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }
    }
}
