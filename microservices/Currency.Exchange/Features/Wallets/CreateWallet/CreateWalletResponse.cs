// Copyright © 2025 Konstantinos Stougiannou

using Currency.Exchange.Common.Endpoints;

namespace Currency.Exchange.Features.Wallets.CreateWallet;

public record CreateWalletResponse : SuccessfulResponse
{
    public long Id { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
