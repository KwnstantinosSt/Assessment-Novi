// Copyright © 2025 Konstantinos Stougiannou

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Currency.Exchange.Common.Microservices;

public static class ConfigurationExtensions
{
    public static void SetupMicroservices(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((_, config) =>
        {
            config.ReadFrom.Configuration(builder.Configuration);
        });

        builder.Configuration
            .SetBasePath(basePath: Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!)
            .AddJsonFile(path: "Config/appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile(path: $"Config/appsettings.{builder.Environment.EnvironmentName}.json",
                optional: true,
                reloadOnChange: true)
            .AddEnvironmentVariables();

        builder.Services.AddOpenApi();
        builder.Services.AddHealthChecks();
        builder.Services.AddHttpContextAccessor();

        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });
    }
}
