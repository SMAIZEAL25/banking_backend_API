
using BankingApp.Application.Interfaces;
using BankingApp.Infrastructure.RateLimiter;
using System.Collections.Concurrent;

namespace BankingApp.Infrastructure.RateLimiter;

public class TokenBucketRateLimiter : IRateLimiter
{
    private readonly ConcurrentDictionary<string, TokenBucket> _buckets;
    private readonly int _maxRequests;
    private readonly double _timeWindowInSeconds;

    public TokenBucketRateLimiter(int maxRequests, double timeWindowInSeconds)
    {
        _buckets = new ConcurrentDictionary<string, TokenBucket>();
        _maxRequests = maxRequests;
        _timeWindowInSeconds = timeWindowInSeconds;
    }

    public bool IsRequestAllowed(string clientIdentifier)
    {
        var bucket = _buckets.GetOrAdd(clientIdentifier,
            _ => new TokenBucket(_maxRequests, _maxRequests / _timeWindowInSeconds));
        return bucket.TryConsume();
    }
}