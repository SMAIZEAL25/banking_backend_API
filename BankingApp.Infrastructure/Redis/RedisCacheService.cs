using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BankingApp.Infrastruture.Redis
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly TimeSpan _defaultExpiry = TimeSpan.FromHours(20); // 20-hour expiry

        public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var data = await _cache.GetStringAsync(key);
                if (data == null)
                {
                    _logger.LogDebug("Cache miss for key: {Key}", key);
                    return default;
                }

                _logger.LogDebug("Cache hit for key: {Key}", key);
                return JsonSerializer.Deserialize<T>(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache item for key: {Key}", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            try
            {
                var actualExpiry = expiry ?? _defaultExpiry;
                var data = JsonSerializer.Serialize(value);

                await _cache.SetStringAsync(key, data, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = actualExpiry
                });

                _logger.LogDebug("Cache set for key: {Key} with expiry: {Expiry}", key, actualExpiry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache item for key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
                _logger.LogDebug("Cache removed for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache item for key: {Key}", key);
            }
        }

        public async Task RefreshAsync(string key)
        {
            try
            {
                await _cache.RefreshAsync(key);
                _logger.LogDebug("Cache refreshed for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing cache item for key: {Key}", key);
            }
        }
    }
}
