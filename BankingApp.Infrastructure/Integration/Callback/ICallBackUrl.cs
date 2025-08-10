using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Infrastruture.Integration.Callback
{
    public interface ICallBackUrl
    {
        Task<string> VerifyPayment(string reference);
    }
}
