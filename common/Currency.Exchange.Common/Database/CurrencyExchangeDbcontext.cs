// Copyright © 2025 Konstantinos Stougiannou

using System.Reflection;
using Currency.Exchange.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Currency.Exchange.Common.Database;

public class CurrencyExchangeDbcontext : DbContext
{
    private readonly string? _schema;

    public CurrencyExchangeDbcontext(DbContextOptions<CurrencyExchangeDbcontext> options, IConfiguration configuration) : base(options)
    {
        _schema = configuration.GetConnectionString(name: "Schema");
    }

     // Database tables
     public DbSet<Wallet> Wallets { get; set; } = null!;
     public DbSet<Models.Currency> Currencies { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        if (!string.IsNullOrEmpty(_schema))
        {
            modelBuilder.HasDefaultSchema(_schema);
        }

        modelBuilder.ApplyConfigurationsFromAssembly(assembly: Assembly.GetExecutingAssembly());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var changedEntries = ChangeTracker.Entries()
            .Where(e => e is { Entity: IEntity, State: EntityState.Added or EntityState.Modified, })
            .ToList();

        var now = DateTime.UtcNow;

        foreach (var entityEntry in changedEntries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                //((IEntity)entityEntry.Entity).Id = GenerateRandomLong();
                ((IEntity)entityEntry.Entity).CreatedAt = DateTime.SpecifyKind(now, DateTimeKind.Utc);
                ((IEntity)entityEntry.Entity).UpdatedAt = DateTime.SpecifyKind(now, DateTimeKind.Utc);

                continue;
            }

            ((IEntity)entityEntry.Entity).UpdatedAt = now;
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    private long GenerateRandomLong()
    {
        return BitConverter.ToInt64(value: Guid.NewGuid().ToByteArray(), 0);
    }
}
