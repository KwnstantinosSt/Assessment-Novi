// Copyright © 2025 Konstantinos Stougiannou

using Currency.Exchange.Common.Cache;
using Currency.Exchange.Common.Database;
using Currency.Exchange.Common.Dto;
using Currency.Exchange.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Exception = System.Exception;
using ILogger = Serilog.ILogger;

namespace Currency.Exchange.Features.Wallets.GetWalletBalance;

public class GetWalletBalanceHandler(CurrencyExchangeDbcontext dbContext, ICacheService cacheService)
{
    private readonly ILogger _logger = Log.ForContext<GetWalletBalanceHandler>();

    public async Task<(GetWalletBalanceResponse, ErrorResponse)> Handle(int walletId, string currency)
    {
        try
        {
            var wallet = await dbContext.Wallets.FirstOrDefaultAsync(w => w.Id == walletId);
            decimal responseAmount = 0;

            if (wallet is null)
            {
                return (new GetWalletBalanceResponse(), new ErrorResponse { IsSuccessful = false, Message = "Wallet not found.", });
            }

            if (wallet.Currency.ToUpper() != currency.ToUpper())
            {
                var rates = await cacheService.GetFromCacheAsync<CurrenciesRatesDto>(key: "latest_rates");

                if (currency.ToUpper() == "EUR")
                {
                    responseAmount = Extensions.ConvertToEuro(wallet.Balance,
                        currency: wallet.Currency.ToUpper(),
                        rates: rates!.CurrenciesRates!);

                    wallet.Currency = "EUR";
                    wallet.Balance = responseAmount;

                    dbContext.Wallets.Update(wallet);
                }
                else if (wallet.Currency.ToUpper() == "EUR")
                {
                    responseAmount = Extensions.ConvertFromEuro(wallet.Balance,
                        currency: currency.ToUpper(),
                        rates: rates!.CurrenciesRates!);

                    wallet.Currency = currency.ToUpper();
                    wallet.Balance = responseAmount;

                    dbContext.Wallets.Update(wallet);
                }
                else
                {
                    // first convert to euro and after to onother currency
                    var responseAmountToEuro = Extensions.ConvertToEuro(wallet.Balance,
                        currency: wallet.Currency.ToUpper(),
                        rates: rates!.CurrenciesRates!);

                    responseAmount = Extensions.ConvertFromEuro(responseAmountToEuro,
                        currency: currency.ToUpper(),
                        rates: rates!.CurrenciesRates!);

                    wallet.Currency = currency.ToUpper();
                    wallet.Balance = responseAmount;

                    dbContext.Wallets.Update(wallet);
                }

                await dbContext.SaveChangesAsync();
            }

            return (new GetWalletBalanceResponse
            {
                Balance = wallet.Balance,
                Currency = wallet.Currency.ToUpper(),
            }, new ErrorResponse { IsSuccessful = true, });
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error converting from wallet balance");

            return (new GetWalletBalanceResponse(), new ErrorResponse { IsSuccessful = false, Message = "Error converting from wallet balance", });
        }
    }
}
