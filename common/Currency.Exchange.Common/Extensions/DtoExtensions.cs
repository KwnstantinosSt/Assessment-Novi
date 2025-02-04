// Copyright © 2025 Konstantinos Stougiannou

using Currency.Exchange.Common.Dto;

namespace Currency.Exchange.Common.Extensions;

public static class DtoExtensions
{
    public static Models.Currency ConvertToCurrency(this CurrenciesRatesDto ratesDto)
    {
        return new Models.Currency
        {
            Id = ratesDto.Id,
            XmlLastUpdateDate = DateTime.SpecifyKind(ratesDto.XmlLastUpdateDate, DateTimeKind.Utc),
            CurrenciesRates = ratesDto.CurrenciesRates,
        };
    }


}
