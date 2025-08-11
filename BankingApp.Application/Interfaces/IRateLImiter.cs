
using System.Collections.Concurrent;


namespace BankingApp.Application.Interfaces
{
    public interface IRateLimiter
    {
        bool IsRequestAllowed(string clientIdentifier);
    }
}

   