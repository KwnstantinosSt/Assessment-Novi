// Copyright © 2025 Konstantinos Stougiannou

using Currency.Exchange.Common.Dto;

namespace Currency.Exchange.Common.Extensions;

public static class Extensions
{
    public static long GenerateRandomLong()
    {
        return BitConverter.ToInt64(value: Guid.NewGuid().ToByteArray(), 0);
    }

    // Method to convert from Euro to another currency
    public static decimal ConvertFromEuro(decimal amount, string currency, List<RatesDto> rates)
    {
        if (rates.Any(r => r.Currency.ToUpper() == currency.ToUpper()))
        {
            var targetCurrencyRate = rates.FirstOrDefault(r => r.Currency == currency.ToUpper())!.Rate;

            return Math.Round(d: amount * targetCurrencyRate, decimals: 2);
        }

        throw new ArgumentException(message: $"Unsupported currency: {currency}");
    }

    // Method to convert from another currency to Euro
    public static decimal ConvertToEuro(decimal amount, string currency, List<RatesDto> rates)
    {
        if (rates.Any(r => r.Currency.ToUpper() == currency.ToUpper()))
        {
            var targetCurrencyRate = rates.FirstOrDefault(r => r.Currency == currency.ToUpper())!.Rate;

            return Math.Round(d: amount / targetCurrencyRate, decimals: 2);
        }

        throw new ArgumentException(message: $"Unsupported currency: {currency}");
    }
}
