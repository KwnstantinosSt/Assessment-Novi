// Copyright © 2025 Konstantinos Stougiannou

using Currency.Exchange.Common.Endpoints;

namespace Currency.Exchange.Responses;

public record WalletResponse : SuccessfulResponse
{
    public string Currency { get; set; }
    public decimal Balance { get; set; }
}
