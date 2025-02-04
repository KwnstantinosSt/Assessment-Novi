// Copyright © 2025 Konstantinos Stougiannou

using Currency.Exchange.Gateway.Configuration;
using Currency.Exchange.Gateway.GatewayBaseClient;
using Microsoft.Extensions.Options;
using Serilog;

namespace Currency.Exchange.Gateway.EuropeanCentralBankClient;

public interface IEuropeanCentralBankClient
{
    Task<T> GetEuropeanBankRates<T>(string requestUri, string cacheKey, CancellationToken cancellationToken = default);
}

public class EuropeanCentralBankClient : IEuropeanCentralBankClient
{
    private readonly IGatewayBaseClient _baseClient;
    private readonly IOptionsSnapshot<GatewayClientConfiguration> _configuration;
    private readonly ILogger _logger = Log.ForContext<EuropeanCentralBankClient>();

    public EuropeanCentralBankClient(IGatewayBaseClient baseClient,
                                     IOptionsSnapshot<GatewayClientConfiguration> configuration)
    {
        _baseClient = baseClient;
        _configuration = configuration;
    }

    public Task<T> GetEuropeanBankRates<T>(string requestUri, string cacheKey, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
