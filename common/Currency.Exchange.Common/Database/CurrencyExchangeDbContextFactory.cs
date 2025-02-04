// Copyright © 2025 Konstantinos Stougiannou

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Currency.Exchange.Common.Database;

public class CurrencyExchangeDbContextFactory : IDesignTimeDbContextFactory<CurrencyExchangeDbcontext>
{
    public CurrencyExchangeDbcontext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var configPath = Path.Combine(Directory.GetCurrentDirectory(), "../../common/Currency.Exchange.Common/Config");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(configPath)
            .AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<CurrencyExchangeDbcontext>();
        var connectionString = configuration.GetConnectionString("Postgres");

        optionsBuilder.UseNpgsql(connectionString);

        return new CurrencyExchangeDbcontext(optionsBuilder.Options, configuration);
    }
}
