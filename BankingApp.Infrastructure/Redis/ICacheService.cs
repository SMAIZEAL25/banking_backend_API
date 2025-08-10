using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Infrastruture.Redis
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task RefreshAsync(string key);
        Task RemoveAsync(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    }
}
