// Copyright © 2025 Konstantinos Stougiannou

using Currency.Exchange.Common.Cache;
using Currency.Exchange.Common.Database;
using Currency.Exchange.Common.Dto;
using Currency.Exchange.Common.Extensions;
using Currency.Exchange.Common.Models;
using Currency.Exchange.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Currency.Exchange.Services;

public interface ICurrencyExchangeService
{
    Task<(T?, ErrorResponse)> CurrencyExchange<T>(int walletId, string currency,
                                                  CancellationToken cancellationToken = default) where T : WalletResponse, new();
}

public class CurrencyExchangeService : ICurrencyExchangeService
{
    private readonly ILogger _logger = Log.ForContext<CurrencyExchangeService>();
    private readonly ICacheService _cacheService;
    private readonly CurrencyExchangeDbcontext _dbContext;
    private IOptionsSnapshot<CacheConfiguration> _cacheOptions;

    public CurrencyExchangeService(ICacheService cacheService, CurrencyExchangeDbcontext dbcontext, IOptionsSnapshot<CacheConfiguration> cacheOptions)
    {
        _cacheService = cacheService;
        _dbContext = dbcontext;
        _cacheOptions = cacheOptions;
    }

    public async Task<(T?, ErrorResponse)> CurrencyExchange<T>(int walletId, string currency, CancellationToken cancellationToken = default)
        where T : WalletResponse, new()
    {
        try
        {
            var wallet = await _dbContext.Wallets.FirstOrDefaultAsync(w => w.Id == walletId, cancellationToken);
            var rates = await _cacheService.GetFromCacheAsync<CurrenciesRatesDto>(key: _cacheOptions.Value.CachingKeys[key: "CurrenciesKey"]);

            if (wallet is null)
            {
                return (null, new ErrorResponse { IsSuccessful = false, Message = "Wallet not found.", });
            }

            var (curr, bal) = await ExchangeOperations(rates: rates, wallet: wallet, currency: currency);

            return (new T
            {
                Balance = bal,
                Currency = curr,
            }, new ErrorResponse { IsSuccessful = true, });
        }
        catch (Exception e)
        {
            _logger.Error(e, messageTemplate: "Error converting from wallet balance");

            return (null, new ErrorResponse { IsSuccessful = false, Message = "Error converting from wallet balance", });
        }
    }

    private async Task<(string, decimal)> ExchangeOperations(CurrenciesRatesDto? rates, Wallet wallet, string currency)
    {
        if (!string.Equals(wallet.Currency, currency, StringComparison.CurrentCultureIgnoreCase))
        {
            decimal responseAmount;

            if (currency.ToUpper() == "EUR")
            {
                responseAmount = Extensions.ConvertToEuro(wallet.Balance,
                    currency: wallet.Currency.ToUpper(),
                    rates: rates!.CurrenciesRates!);

                wallet.Currency = "EUR";
                wallet.Balance = responseAmount;
            }
            else if (wallet.Currency.ToUpper() == "EUR")
            {
                responseAmount = Extensions.ConvertFromEuro(wallet.Balance,
                    currency: currency.ToUpper(),
                    rates: rates!.CurrenciesRates!);

                wallet.Currency = currency.ToUpper();
                wallet.Balance = responseAmount;
            }
            else
            {
                // first convert to euro and after to another currency
                var responseAmountToEuro = Extensions.ConvertToEuro(wallet.Balance,
                    currency: wallet.Currency.ToUpper(),
                    rates: rates!.CurrenciesRates!);

                responseAmount = Extensions.ConvertFromEuro(responseAmountToEuro,
                    currency: currency.ToUpper(),
                    rates: rates!.CurrenciesRates!);

                wallet.Currency = currency.ToUpper();
                wallet.Balance = responseAmount;
            }
        }

        _dbContext.Wallets.Update(wallet);
        await _dbContext.SaveChangesAsync();

        return (wallet.Currency, wallet.Balance);
    }
}
