// Copyright © 2025 Konstantinos Stougiannou

using Currency.Exchange.Common.Endpoints;

namespace Currency.Exchange.Features.Wallets.EditWalletBalance;

public record EditWalletBalanceResponse : SuccessfulResponse
{
    public string Currency { get; set; }
    public decimal Balance { get; set; }
}
