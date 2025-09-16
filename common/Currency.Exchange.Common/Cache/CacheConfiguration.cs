// Copyright © 2025 Konstantinos Stougiannou

namespace Currency.Exchange.Common.Cache;

public class CacheConfiguration
{
    public required string Namespace { get; init; }

    public TimeSpan? ExpirationInSeconds { get; init; }
    public Dictionary<string, string> CachingKeys { get; init; }
}

