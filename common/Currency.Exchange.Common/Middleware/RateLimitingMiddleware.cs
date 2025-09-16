// Copyright © 2025 Konstantinos Stougiannou


using System.Collections.Concurrent;
using Currency.Exchange.Common.Endpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Currency.Exchange.Common.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private static readonly ConcurrentDictionary<string, int> _requestCounts = new();
    private static readonly TimeSpan _timeWindow = TimeSpan.FromMinutes(1);
    private const int _requestLimit = 10;

    public RateLimitingMiddleware(RequestDelegate next, IMemoryCache cache)
    {
        _next = next;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        // Retrieve or initialize request count
        if (!_cache.TryGetValue(ip, out _))
        {
            _requestCounts[ip] = 0; // Reset count
            _cache.Set(ip, true, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _timeWindow,
                PostEvictionCallbacks = { new PostEvictionCallbackRegistration
                {
                    EvictionCallback = (key, value, reason, state) =>
                    {
                        _requestCounts.TryRemove(key.ToString(), out _); // Clear request count when cache expires
                    }
                }}
            });
        }

        _requestCounts.AddOrUpdate(ip, 1, (_, count) => count + 1);

        if (_requestCounts[ip] > _requestLimit)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;

            await context.Response.WriteAsJsonAsync(value: new ErrorResponse
            {
                Message = "Too many requests!",
                Code = "400",
            });

            return;
        }

        await _next(context);
    }
}
