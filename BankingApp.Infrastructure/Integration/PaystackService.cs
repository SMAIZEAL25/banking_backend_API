
using BankingApp.Infrastruture.Integration.Response;
using BankingApp.Infrastruture.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Infrastruture.Integration
{
    public class PaystackService : IPaymentGateway
    {
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;
        private readonly ILogger<PaystackService> _logger;

        public PaystackService(
            HttpClient httpClient,
            IConfiguration config,
            ILogger<PaystackService> logger)
        {
            _httpClient = httpClient;
            _secretKey = config["Paystack:SecretKey"];
            _logger = logger;
        }

        public async Task<PaystackTransactionResponse> InitializeTransaction(PaymentRequest request)
        {
            // Mocked response for testing
            if (_secretKey == "test_key")
            {
                return new PaystackTransactionResponse
                {
                    Status = true,
                    Message = "Authorization URL created",
                    Data = new TransactionData
                    {
                        AuthorizationUrl = "https://paystack.mock/pay/" + request.Reference,
                        AccessCode = "mock_access_code",
                        Reference = request.Reference
                    }
                };
            }

            // Convert Naira → Kobo before sending to Paystack
            var paystackPayload = new
            {
                amount = (int)(request.Amount * 100), // Paystack expects integer in kobo
                email = request.Email,
                reference = request.Reference,
                callback_url = request.CallbackUrl,
                currency = request.Currency,
                metadata = request.Metadata
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(paystackPayload),
                Encoding.UTF8,
                "application/json");

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _secretKey);

            var response = await _httpClient.PostAsync(
                "https://api.paystack.co/transaction/initialize",
                content);

            return await response.Content.ReadFromJsonAsync<PaystackTransactionResponse>();
        }
    }
}



//[ApiController]
//[Route("api/webhooks/paystack")]
//public class PaystackWebhookController : ControllerBase
//{
//    private readonly ILogger<PaystackWebhookController> _logger;
//    private readonly IPaymentService _paymentService;

//    public PaystackWebhookController(
//        ILogger<PaystackWebhookController> logger,
//        IPaymentService paymentService)
//    {
//        _logger = logger;
//        _paymentService = paymentService;
//    }

//    [HttpPost]
//    public async Task<IActionResult> HandleWebhook()
//    {
//        var json = await new StreamReader(Request.Body).ReadToEndAsync();
//        var signature = Request.Headers["x-paystack-signature"];

//        // Verify signature here (omitted for brevity)

//        var webhookEvent = JsonConvert.DeserializeObject<PaystackWebhookEvent>(json);

//        _logger.LogInformation($"Received Paystack webhook: {webhookEvent.Event}");

//        if (webhookEvent.Event == "charge.success")
//        {
//            await _paymentService.ProcessSuccessfulPayment(
//                webhookEvent.Data.Reference,
//                webhookEvent.Data.Amount);
//        }

//        return Ok();
//    }
//}