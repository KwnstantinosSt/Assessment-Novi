// Copyright © 2025 Konstantinos Stougiannou

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Currency.Exchange.Common.Models;

public class Wallet : IEntity
{
    public long Id { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; } = String.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.HasKey(w => w.Id);
        builder.Property(W => W.Currency)
            .HasDefaultValue(value: "EUR");
    }
}
