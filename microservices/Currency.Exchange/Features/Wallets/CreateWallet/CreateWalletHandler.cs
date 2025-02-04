// Copyright © 2025 Konstantinos Stougiannou

using Currency.Exchange.Common.Database;
using Currency.Exchange.Common.Dto;
using Currency.Exchange.Common.Models;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Currency.Exchange.Features.Wallets.CreateWallet;

public class CreateWalletHandler(CurrencyExchangeDbcontext dbContext)
{
    private readonly ILogger _logger = Log.ForContext<CreateWalletHandler>();

    public async Task<(CreateWalletResponse, ErrorResponse)> Handle()
    {
        try
        {
            var newWallet = new Wallet
            {
                Currency = "EUR",
                Balance = 0,
            };

            var wallet = await dbContext.Wallets.AddAsync(newWallet);
            await dbContext.SaveChangesAsync();

            return (new CreateWalletResponse
            {
                Id = wallet.Entity.Id,
                Currency = newWallet.Currency,
                Balance = wallet.Entity.Balance,
                CreatedAt = wallet.Entity.CreatedAt,
                UpdatedAt = wallet.Entity.UpdatedAt,
            }, new ErrorResponse
            {
                IsSuccessful = true,
            });
        }
        catch (Exception e)
        {
            _logger.Error(e, e.Message);
            return (new CreateWalletResponse(), new ErrorResponse { IsSuccessful = false, Message = "Error creating new wallet", });
        }
    }
}
