// Copyright © 2025 Konstantinos Stougiannou

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Currency.Exchange.Common.Cache;
using Currency.Exchange.Common.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StackExchange.Redis;

namespace Currency.Exchange.Common.Microservices;

public static class ConfigurationExtensions
{
    public static void SetupMicroservices(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((_, config) =>
        {
            config.ReadFrom.Configuration(builder.Configuration);
        });
        
        builder.Services.AddMemoryCache();

        builder.Configuration
            .SetBasePath(basePath: Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!)
            .AddJsonFile(path: "Config/appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile(path: $"Config/appsettings.{builder.Environment.EnvironmentName}.json",
                optional: true,
                reloadOnChange: true)
            .AddEnvironmentVariables();

        builder.Services.AddSingleton<IConnectionMultiplexer>(
            implementationInstance: ConnectionMultiplexer.Connect(
                configuration: builder.Configuration.GetConnectionString(name: "Redis")!));

        builder.Services.AddDbContext<CurrencyExchangeDbcontext>(options =>
            options.UseNpgsql(connectionString: builder.Configuration.GetConnectionString(name: "Postgres"),
                x => x.MigrationsHistoryTable(tableName: "migrations_history",
                    schema: builder.Configuration.GetConnectionString(name: "Schema"))));

        builder.Services.Configure<CacheConfiguration>(config: builder.Configuration.GetSection(key: "Caching"));

        builder.Services.AddOpenApi();
        builder.Services.AddHealthChecks();
        builder.Services.AddHttpContextAccessor();

        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        builder.Services.AddScoped<ICacheService, CacheService>();
    }
}
