using BankingApp.Infrastruture.Integration.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Infrastruture.Services.Interfaces
{
    public interface IPaymentGateway
    {
        Task<PaystackTransactionResponse> InitializeTransaction(PaymentRequest request);
    }
}
