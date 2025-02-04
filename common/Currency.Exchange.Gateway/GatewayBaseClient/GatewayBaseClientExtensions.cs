// Copyright © 2025 Konstantinos Stougiannou

using Currency.Exchange.Gateway.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace Currency.Exchange.Gateway.GatewayBaseClient;

public static class GatewayBaseClientExtensions
{
    public static IHttpClientBuilder AddGatewayClient(this IServiceCollection services, bool enableRetryPolicy = true)
    {
        var builder = services.AddHttpClient<IGatewayBaseClient, GatewayBaseClient>();

        if (enableRetryPolicy)
        {
            return builder
                .AddTransientHttpErrorPolicy(p =>
                    p.WaitAndRetryAsync(
                        sleepDurations: Backoff.DecorrelatedJitterBackoffV2(
                            medianFirstRetryDelay: TimeSpan.FromSeconds(value: 1),
                            retryCount: 3
                        )
                    )
                );
        }

        return builder;
    }
}
