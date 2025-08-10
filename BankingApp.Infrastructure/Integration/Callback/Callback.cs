using BankingApp.Infrastruture.Integration.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Infrastruture.Integration.Callback
{
    public class CallBackUrl : ICallBackUrl
    {
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;

        public CallBackUrl(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _secretKey = config["Paystack:SecretKey"];
        }

        public async Task<string> VerifyPayment(string reference)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"https://api.paystack.co/transaction/verify/{reference}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _secretKey);

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Paystack verification failed: {content}");
            }

            var result = JsonConvert.DeserializeObject<PaystackVerificationResponse>(content);
            return result.Data.Status;
        }
    }
}

//[ApiController]
//[Route("api/payments")]
//public class PaymentCallbackController : ControllerBase
//{
//    private readonly IPaymentService _paymentService;
//    private readonly ILogger<PaymentCallbackController> _logger;

//    public PaymentCallbackController(
//        IPaymentService paymentService,
//        ILogger<PaymentCallbackController> logger)
//    {
//        _paymentService = paymentService;
//        _logger = logger;
//    }

//    [HttpGet("verify/{reference}")]
//    public async Task<IActionResult> HandlePaymentCallback(string reference)
//    {
//        try
//        {
//            _logger.LogInformation($"Received payment callback for reference: {reference}");

//            // Verify payment with Paystack
//            var paymentStatus = await _paymentService.VerifyPayment(reference);

//            // Handle different status cases
//            return paymentStatus switch
//            {
//                "success" => Ok(new
//                {
//                    status = "success",
//                    message = "Payment verified successfully",
//                    reference = reference
//                }),
//                "pending" => Accepted(new
//                {
//                    status = "pending",
//                    message = "Payment processing",
//                    reference = reference
//                }),
//                _ => BadRequest(new
//                {
//                    status = "failed",
//                    message = "Payment verification failed",
//                    reference = reference
//                })
//            };
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, $"Error processing callback for reference: {reference}");
//            return StatusCode(500, new
//            {
//                status = "error",
//                message = "Internal server error",
//                reference = reference
//            });
//        }
//    }
//}

//[HttpPost("webhook")]
//public async Task<IActionResult> HandleWebhook([FromBody] PaystackWebhookEvent webhookEvent)
//{
//    if (webhookEvent.Event == "charge.success")
//    {
//        await _paymentService.ProcessSuccessfulPayment(webhookEvent.Data.Reference);
//    }
//    return Ok();
//}