// Copyright © 2025 Konstantinos Stougiannou

namespace Currency.Exchange.Common.Dto;

public class RatesDto
{
    public string Currency { get; set; } = string.Empty;
    public decimal Rate { get; set; }
}
