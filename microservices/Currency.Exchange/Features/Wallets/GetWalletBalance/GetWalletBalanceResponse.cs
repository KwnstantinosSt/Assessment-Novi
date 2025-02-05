// Copyright © 2025 Konstantinos Stougiannou

using Currency.Exchange.Common.Endpoints;

namespace Currency.Exchange.Features.Wallets.GetWalletBalance;

public record GetWalletBalanceResponse : SuccessfulResponse
{
    public string Currency { get; set; }
    public decimal Balance { get; set; }
}
