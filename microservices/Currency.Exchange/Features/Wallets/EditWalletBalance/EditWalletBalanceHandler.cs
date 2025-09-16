// Copyright © 2025 Konstantinos Stougiannou

using Currency.Exchange.Common.Database;
using Currency.Exchange.Common.Dto;
using Currency.Exchange.Common.Models;
using Currency.Exchange.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Currency.Exchange.Features.Wallets.EditWalletBalance;

public class EditWalletBalanceHandler(ICurrencyExchangeService currencyExchangeService, CurrencyExchangeDbcontext dbcontext)
{
    private readonly ILogger _logger = Log.ForContext<EditWalletBalanceHandler>();

    public async Task<(EditWalletBalanceResponse, ErrorResponse)> Handle(int walletId, string currency, string strategy,
                                                                         decimal amount, CancellationToken cancellationToken)
    {
        try
        {
            var wallet = await dbcontext.Wallets.FirstOrDefaultAsync(w => w.Id == walletId, cancellationToken);

            if (wallet is null) return (new EditWalletBalanceResponse(), new ErrorResponse { Message = "Wallet not found", IsSuccessful = false, });

            var (transaction, error) = await BalanceTransaction(wallet, strategy, amount);

            if (!error.IsSuccessful) return (new EditWalletBalanceResponse(), error);

            dbcontext.Wallets.Update(transaction);
            await dbcontext.SaveChangesAsync(cancellationToken);

            var (resp, err) =
                await currencyExchangeService.CurrencyExchange<EditWalletBalanceResponse>(
                    walletId: walletId!,
                    currency,
                    cancellationToken: cancellationToken);

            if (err.IsSuccessful)
            {
                return (resp!, err);
            }
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error converting from wallet balance");

            return (new EditWalletBalanceResponse(),
                new ErrorResponse { Message = e.Message, IsSuccessful = false, });
        }

        return (new EditWalletBalanceResponse(),
            new ErrorResponse { Message = "Error converting from wallet balance", IsSuccessful = false, });
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
                    var negativeAmount = decimal.Negate(d: amount - wallet.Balance);
                    wallet.Balance = negativeAmount;
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
