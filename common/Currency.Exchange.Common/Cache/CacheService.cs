// Copyright © 2025 Konstantinos Stougiannou

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Currency.Exchange.Common.Cache;

public interface ICacheService
{
    Task<T?> GetFromCacheAsync<T>(string key);
    Task Invalidate(string key, bool fireAndForget = true);
    Task Set<T>(string key, T data, bool fireAndForget = true);

}

public class CacheService : ICacheService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly IConnectionMultiplexer _cache;
    private readonly IOptionsSnapshot<CacheConfiguration> _cacheConfiguration;

    public CacheService(IConnectionMultiplexer connectionMultiplexer,
                        IOptionsSnapshot<CacheConfiguration> cacheConfiguration)
    {
        _cache = connectionMultiplexer;
        _cacheConfiguration = cacheConfiguration;
    }

    public async Task<T?> GetFromCacheAsync<T>(string key)
    {
        var db = _cache.GetDatabase();

        string? cachedDataJson = await db.StringGetAsync(key:
            $"{_cacheConfiguration.Value.Namespace}:{key}");

        return cachedDataJson != null
            ? JsonSerializer.Deserialize<T>(cachedDataJson, JsonOptions)
            : default;
    }

    public async Task Invalidate(string key, bool fireAndForget = true)
    {
        var db = _cache.GetDatabase();

        await db.KeyDeleteAsync(key: $"{_cacheConfiguration.Value.Namespace}:{key}",
            flags: fireAndForget
                ? CommandFlags.FireAndForget
                : CommandFlags.None);
    }

    public async Task Set<T>(string key, T data, bool fireAndForget = true)
    {
        var db = _cache.GetDatabase();

        await db.StringSetAsync(key: $"{_cacheConfiguration.Value.Namespace}:{key}",
            value: JsonSerializer.Serialize(data, JsonOptions),
            expiry: _cacheConfiguration.Value.ExpirationInSeconds ?? null,
            flags: fireAndForget ? CommandFlags.FireAndForget : CommandFlags.None);
    }
}
