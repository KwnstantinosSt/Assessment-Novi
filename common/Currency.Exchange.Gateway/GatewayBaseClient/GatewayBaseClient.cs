// Copyright © 2025 Konstantinos Stougiannou

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Currency.Exchange.Common.Cache;
using Currency.Exchange.Gateway.Configuration;
using Microsoft.Extensions.Options;
using Serilog;

namespace Currency.Exchange.Gateway.GatewayBaseClient;

public interface IGatewayBaseClient
{
    Task<GatewayClientResult<T>> GetXmlFromService<T>(
        string requestUri,
        CancellationToken cancellationToken = default);

    public GatewayClientConfiguration Configuration { get; }
}

public class GatewayBaseClient : IGatewayBaseClient
{
    private readonly HttpClient _httpClient;
    private readonly IOptionsSnapshot<GatewayClientConfiguration> _configuration;
    private readonly ILogger _logger = Log.ForContext<GatewayBaseClient>();

    public GatewayBaseClient(HttpClient httpClient, IOptionsSnapshot<GatewayClientConfiguration> configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public GatewayClientConfiguration Configuration => _configuration.Value;

    public async Task<GatewayClientResult<T>> GetXmlFromService<T>(
        string requestUri, CancellationToken cancellationToken = default)
    {
        var message = new HttpRequestMessage(HttpMethod.Get, requestUri);

        string? responseText = null;

        try
        {
            var response = await _httpClient.SendAsync(message, cancellationToken);
            responseText = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await DeserializeXml<T>(responseText);

                return new GatewayClientResult<T>
                {
                    IsSuccessful = true,
                    Data = responseData,
                };
            }

            _logger.Error(
                messageTemplate: "Sending {Method} {Uri} failed with empty response",
                message.Method,
                message.RequestUri);

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

    private async Task<T> DeserializeXml<T>(string xml)
    {
        using var reader = new StringReader(xml);

        var serializer = new XmlSerializer(type: typeof(T));

        return ((T)serializer.Deserialize(reader)!)!;
    }
}
