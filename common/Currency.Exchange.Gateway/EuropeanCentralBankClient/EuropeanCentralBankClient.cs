// Copyright © 2025 Konstantinos Stougiannou

using System.Text.Json;
using System.Text.Json.Serialization;
using Currency.Exchange.Common.Database;
using Currency.Exchange.Common.Dto;
using Currency.Exchange.Gateway.Configuration;
using Currency.Exchange.Gateway.GatewayBaseClient;
using Currency.Exchange.Gateway.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;

namespace Currency.Exchange.Gateway.EuropeanCentralBankClient;

public interface IEuropeanCentralBankClient
{
    Task<GatewayClientResult<CurrenciesRatesDto>> GetEuropeanBankRates(CancellationToken cancellationToken = default);
}

public class EuropeanCentralBankClient : IEuropeanCentralBankClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly IGatewayBaseClient _baseClient;
    private readonly ILogger _logger = Log.ForContext<EuropeanCentralBankClient>();

    public EuropeanCentralBankClient(IGatewayBaseClient baseClient)
    {
        _baseClient = baseClient;
    }

    public async Task<GatewayClientResult<CurrenciesRatesDto>> GetEuropeanBankRates(CancellationToken cancellationToken = default)
    {
        var response = await _baseClient.GetXmlFromService<EcbEnvelope>(
            requestUri: _baseClient.Configuration.GetEuropaUrl,
            cancellationToken: cancellationToken);

        if (!response.IsSuccessful)
        {
            return new GatewayClientResult<CurrenciesRatesDto>
            {
                IsSuccessful = false,
            };
        }

        return new GatewayClientResult<CurrenciesRatesDto>
        {
            IsSuccessful = true,
            Data = new CurrenciesRatesDto
            {
                Id = GenerateRandomLong(),
                CurrenciesRates = response.Data!.CubeWrapper.DateCube.CurrencyRates
                    .Select(cw => new RatesDto { Currency = cw.Currency, Rate = cw.Rate, })
                    .ToList(),
                XmlLastUpdateDate = DateTime.Parse(response.Data!.CubeWrapper.DateCube.Time),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            },
        };
    }

    private long GenerateRandomLong()
    {
        return BitConverter.ToInt64(value: Guid.NewGuid().ToByteArray(), startIndex: 0);
    }
}
