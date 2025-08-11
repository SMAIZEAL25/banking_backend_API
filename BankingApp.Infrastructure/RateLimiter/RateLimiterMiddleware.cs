using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Threading.RateLimiting;


namespace BankingApp.Infrastructure.RateLimiter
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRateLimiter _rateLimiter;

        public RateLimitingMiddleware(RequestDelegate next, IRateLimiter rateLimiter)
        {
            _next = next;
            _rateLimiter = rateLimiter;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Get client identifier (could be IP, API key, etc.)
            var clientIdentifier = context.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrEmpty(clientIdentifier))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            if (!_rateLimiter.IsRequestAllowed(clientIdentifier))
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Too many request exceeded. Please try again later.");
                return;
            }

            await _next(context);
        }
    }
}
