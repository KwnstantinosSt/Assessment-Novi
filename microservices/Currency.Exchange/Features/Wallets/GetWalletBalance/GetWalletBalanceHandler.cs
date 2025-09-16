// Copyright © 2025 Konstantinos Stougiannou

using System.Text.Json;
using Currency.Exchange.Common.Dto;
using Currency.Exchange.Services;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Currency.Exchange.Features.Wallets.GetWalletBalance;

public class GetWalletBalanceHandler(ICurrencyExchangeService currencyExchangeService)
{
    private readonly ILogger _logger = Log.ForContext<GetWalletBalanceHandler>();

    public async Task<(GetWalletBalanceResponse?, ErrorResponse)> Handle(int walletId, string currency, CancellationToken cancellation)
    {
        var (response, errorResponse) = await currencyExchangeService.CurrencyExchange<GetWalletBalanceResponse>(walletId: walletId,
            currency: currency,
            cancellationToken: cancellation);

        if (!errorResponse.IsSuccessful)
        {
            _logger.Information(messageTemplate: "Get wallet balance response: {response}",
                propertyValue: JsonSerializer.Serialize(errorResponse));

            return (new GetWalletBalanceResponse(), errorResponse);
        }

        _logger.Information(messageTemplate: "Get wallet balance response: {response}",
            propertyValue: JsonSerializer.Serialize(response));

        return (response, errorResponse);
    }
}
