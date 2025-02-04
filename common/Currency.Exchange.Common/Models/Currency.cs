// Copyright © 2025 Konstantinos Stougiannou

using System.Text.Json;
using Currency.Exchange.Common.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Currency.Exchange.Common.Models;

public class Currency : IEntity
{
    public required long Id { get; set; }
    public DateTime XmlLastUpdateDate { get; set; }
    public CurrenciesRatesDto? CurrenciesRates { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}


public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.CurrenciesRates)
            .HasColumnType("jsonb")
            .HasConversion(
                j => JsonSerializer.Serialize(j, (JsonSerializerOptions)null!),
                j => JsonSerializer.Deserialize<CurrenciesRatesDto>(j, (JsonSerializerOptions)null!)
            );
    }
}
