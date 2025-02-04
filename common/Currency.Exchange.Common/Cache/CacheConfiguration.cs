// Copyright © 2025 Konstantinos Stougiannou

namespace Currency.Exchange.Common.Cache;

public class CacheConfiguration
{
    public required string Namespace { get; set; }

    public TimeSpan? ExpirationInSeconds { get; set; }
}
