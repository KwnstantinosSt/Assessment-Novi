// Copyright © 2025 Konstantinos Stougiannou

namespace Currency.Exchange.Common.Dto;

public class CurrenciesRatesDto
{
    public required long Id { get; set; }
    public DateTime XmlLastUpdateDate { get; set; }
    public List<RatesDto>? CurrenciesRates { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
