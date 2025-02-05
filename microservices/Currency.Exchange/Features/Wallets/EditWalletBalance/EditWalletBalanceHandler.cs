// Copyright © 2025 Konstantinos Stougiannou

using Currency.Exchange.Common.Cache;
using Currency.Exchange.Common.Database;
using Currency.Exchange.Common.Dto;
using Currency.Exchange.Common.Extensions;
using Currency.Exchange.Common.Models;
using Currency.Exchange.Features.Wallets.GetWalletBalance;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Currency.Exchange.Features.Wallets.EditWalletBalance;

public class EditWalletBalanceHandler(CurrencyExchangeDbcontext dbContext, ICacheService cacheService)
{
    private readonly ILogger _logger = Log.ForContext<EditWalletBalanceHandler>();

    public async Task<(EditWalletBalanceResponse, ErrorResponse)> Handle(int walletId, string currency, string strategy, decimal amount)
    {
        try
        {
            var wallet = await dbContext.Wallets.FirstOrDefaultAsync(w => w.Id == walletId);

            if (wallet is null)
            {
                return (new EditWalletBalanceResponse(), new ErrorResponse { IsSuccessful = false, Message = "Wallet not found.", });
            }

            var (response, error) = await BalanceTransaction(wallet: wallet, amount: amount, strategy: strategy);

            decimal responseAmount;

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
            }

            await dbContext.SaveChangesAsync();

            return (new EditWalletBalanceResponse
            {
                Balance = wallet.Balance,
                Currency = wallet.Currency.ToUpper(),
            }, new ErrorResponse { IsSuccessful = true, });
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error converting from wallet balance");

            return (new EditWalletBalanceResponse(), new ErrorResponse { IsSuccessful = false, Message = "Error converting from wallet balance", });
        }
    }


    private async Task<(Wallet, ErrorResponse)> BalanceTransaction(
        Wallet wallet, string strategy, decimal amount)
    {
        try
        {
            switch (strategy.ToUpper())
            {
                case "ADDFUNDSSTRATEGY":
                    wallet.Balance += amount;
                    break;

                case "SUBSTRACTFUNDSSTRATEGY":
                    if (wallet.Balance < amount)
                    {
                        return (wallet,
                            new ErrorResponse { IsSuccessful = false, Message = "Insufficient funds in the wallet.", });
                    }

                    wallet.Balance -= amount;
                    break;

                case "FORCESUBSTRACTFUNDSSTRATEGY":
                    wallet.Balance -= amount;
                    break;

                default:
                    return (wallet,
                        new ErrorResponse { IsSuccessful = false, Message = "Insufficient funds in the wallet.", });
            }
        }
        catch (Exception e)
        {
            _logger.Error(e, messageTemplate: "Error converting wallet balance");
        }

        return (wallet,
            new ErrorResponse { IsSuccessful = true, });
    }
}
