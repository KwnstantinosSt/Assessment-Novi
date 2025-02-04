// Copyright © 2025 Konstantinos Stougiannou

using System.Text.Json;
using System.Text.Json.Serialization;
using Currency.Exchange.Common.Cache;
using Currency.Exchange.Gateway.Configuration;
using Microsoft.Extensions.Options;
using Serilog;

namespace Currency.Exchange.Gateway.GatewayBaseClient;

public interface IGatewayBaseClient
{
    Task<GatewayClientResult<T>> GetCachedOrGetFromService<T>(
        string requestUri,
        string cacheKey,
        CancellationToken cancellationToken = default);
}

public class GatewayBaseClient : IGatewayBaseClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly HttpClient _httpClient;
    private readonly ICacheService _cacheService;
    private readonly ILogger _logger = Log.ForContext<GatewayBaseClient>();

    public GatewayBaseClient(HttpClient httpClient, ICacheService cacheService)
    {
        _httpClient = httpClient;
        _cacheService = cacheService;
    }

    public async Task<GatewayClientResult<T>> GetCachedOrGetFromService<T>(
        string requestUri, string cacheKey, CancellationToken cancellationToken = default)
    {
        var cachedData = await _cacheService.GetFromCacheAsync<T>(key: cacheKey);

        if (cachedData != null)
        {
            return new GatewayClientResult<T>
            {
                IsSuccessful = true,
                Data = cachedData,
            };
        }

        var message = new HttpRequestMessage(HttpMethod.Get, requestUri);
        string? responseText = null;

        try
        {
            var response = await _httpClient.SendAsync(message, cancellationToken);
            responseText = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseData = JsonSerializer.Deserialize<T>(responseText, JsonOptions);

                await _cacheService.Set<T>(cacheKey, data: responseData!);

                return new GatewayClientResult<T>
                {
                    IsSuccessful = true,
                    Data = responseData,
                };
            }

            return new GatewayClientResult<T>
            {
                IsSuccessful = false,
            };
        }
        catch (Exception e)
        {
            _logger.Error(e,
                messageTemplate: "Sending {Method} {Uri} failed with {Response}",
                message.Method,
                message.RequestUri,
                responseText);

            return new GatewayClientResult<T>
            {
                IsSuccessful = false,
            };
        }
    }
}
