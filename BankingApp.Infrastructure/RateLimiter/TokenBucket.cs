namespace IRecharge_API1.RateLimiter
{
    public class TokenBucket
    {
        private readonly int _capacity;
        private readonly double _refillRatePerSecond;
        private double _currentTokens;
        private DateTime _lastRefillTime;

        public TokenBucket(int capacity, double refillRatePerSecond)
        {
            _capacity = capacity;
            _refillRatePerSecond = refillRatePerSecond;
            _currentTokens = capacity;
            _lastRefillTime = DateTime.UtcNow;
        }

        public bool TryConsume()
        {
            Refill();

            if (_currentTokens >= 1)
            {
                _currentTokens -= 1;
                return true;
            }

            return false;
        }

        private void Refill()
        {
            var now = DateTime.UtcNow;
            var timeElapsed = (now - _lastRefillTime).TotalSeconds;
            var tokensToAdd = timeElapsed * _refillRatePerSecond;

            _currentTokens = Math.Min(_capacity, _currentTokens + tokensToAdd);
            _lastRefillTime = now;
        }
    }
}
