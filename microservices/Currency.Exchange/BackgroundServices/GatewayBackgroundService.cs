// Copyright © 2025 Konstantinos Stougiannou

using System.Text.Json;
using Currency.Exchange.Common.Cache;
using Currency.Exchange.Common.Database;
using Currency.Exchange.Common.Extensions;
using Currency.Exchange.Gateway.EuropeanCentralBankClient;
using Microsoft.EntityFrameworkCore;
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
        _logger.Information("Running Gateway background service");
        var response = await _client.GetEuropeanBankRates();

        if (response.IsSuccessful && response.Data != null)
        {
            var entity = await _context.Currencies.AddAsync(entity: response.Data.ConvertToCurrency());
            await _context.SaveChangesAsync();
            response.Data.Id = entity.Entity.Id;
            await _cacheService.Set(key: "latest_rates", response.Data);
        }

        _logger.Information(messageTemplate: JsonSerializer.Serialize(response.Data));
    }
}
