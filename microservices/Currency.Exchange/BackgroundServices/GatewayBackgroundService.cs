// Copyright © 2025 Konstantinos Stougiannou

using System.Text.Json;
using Currency.Exchange.Common.Cache;
using Currency.Exchange.Common.Database;
using Currency.Exchange.Common.Extensions;
using Currency.Exchange.Gateway.EuropeanCentralBankClient;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using Quartz;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Currency.Exchange.BackgroundServices;

public class GatewayBackgroundService : IJob
{
    private readonly ILogger _logger = Log.ForContext<GatewayBackgroundService>();
    private readonly CurrencyExchangeDbcontext _context;
    private readonly ICacheService _cacheService;
    private readonly IEuropeanCentralBankClient _client;

    public GatewayBackgroundService(CurrencyExchangeDbcontext context,
                                    ICacheService cacheService,
                                    IEuropeanCentralBankClient client)
    {
        _context = context;
        _cacheService = cacheService;
        _client = client;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.Information(messageTemplate: "Running Gateway background service");

        await CreateOrUpdateDbAndCache();
    }

    private async Task CreateOrUpdateDbAndCache()
    {
        try
        {
            var response = await _client.GetEuropeanBankRates();

            if (response.IsSuccessful)
            {
                _logger.Information(messageTemplate: JsonSerializer.Serialize(response.Data));

                var sql = @"MERGE INTO ""Currencies"" AS c
                    USING (SELECT @Id AS id,
                                  @XmlLastUpdateDate AS xmlLastUpdateDate,
                                  @CurrenciesRates AS currenciesRates,
                                  @CreatedAt AS createdAt,
                                  @UpdatedAt AS updatedAt) AS t
                    ON c.""XmlLastUpdateDate"" = t.xmlLastUpdateDate
                    WHEN MATCHED THEN
                        UPDATE SET ""XmlLastUpdateDate"" = t.xmlLastUpdateDate,
                                   ""CurrenciesRates"" = t.currenciesRates,
                                   ""UpdatedAt"" = t.updatedAt
                    WHEN NOT MATCHED THEN
                        INSERT (""Id"", ""XmlLastUpdateDate"", ""CurrenciesRates"", ""CreatedAt"", ""UpdatedAt"")
                        VALUES (t.id, t.xmlLastUpdateDate, t.currenciesRates, t.createdAt, t.updatedAt);";

                var parameters = new[]
                {
                    new NpgsqlParameter(parameterName: "@Id", value: response.Data?.Id ?? (object)DBNull.Value),
                    new NpgsqlParameter(parameterName: "@XmlLastUpdateDate", value: response.Data?.XmlLastUpdateDate ?? (object)DBNull.Value),
                    new NpgsqlParameter(parameterName: "@CurrenciesRates", NpgsqlDbType.Jsonb) { Value = JsonSerializer.Serialize(response.Data?.CurrenciesRates) },
                    new NpgsqlParameter(parameterName: "@CreatedAt", value: response.Data?.CreatedAt ?? (object)DBNull.Value),
                    new NpgsqlParameter(parameterName: "@UpdatedAt", value: response.Data?.UpdatedAt ?? (object)DBNull.Value),
                };


                // Save to db
                await _context.Database.ExecuteSqlRawAsync(sql, parameters: parameters.Cast<object>().ToArray());
                // Set latest update to cache
                await _cacheService.Set(key: "latest_rates", response.Data);
                _logger.Information(messageTemplate: "Data updated successfully db and cache");

                return;
            }

            _logger.Error(messageTemplate: "Client response with error.");
        }
        catch (Exception e)
        {
            _logger.Error(e, messageTemplate: "An error ocurred while updating the database or cache.");
        }
    }
}
