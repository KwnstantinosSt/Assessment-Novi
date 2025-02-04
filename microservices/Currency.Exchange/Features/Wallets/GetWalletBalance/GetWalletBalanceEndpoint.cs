// Copyright © 2025 Konstantinos Stougiannou

using Currency.Exchange.Common.Dto;
using Currency.Exchange.Common.Endpoints;
using Currency.Exchange.Gateway.EuropeanCentralBankClient;
using Currency.Exchange.Gateway.GatewayBaseClient;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Currency.Exchange.Features.Wallets.GetWalletBalance;

public class GetWalletBalanceEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(pattern: "v1/currencies", GetCurrencies)
            .WithSummary("Returns a wallet balance")
            .WithName(endpointName: "GetWeatherForecast2");
    }

    public async Task<Ok<SuccessfulResponseWithData<GatewayClientResult<CurrenciesRatesDto>>>> GetCurrencies(
        [FromServices] IEuropeanCentralBankClient client)
    {
        var response = await client.GetEuropeanBankRates();

        return TypedResults.Extensions.SuccessWithData(response);
    }
}
