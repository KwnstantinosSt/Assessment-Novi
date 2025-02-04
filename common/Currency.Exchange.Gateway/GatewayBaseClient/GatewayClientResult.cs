// Copyright © 2025 Konstantinos Stougiannou

using System.Net;

namespace Currency.Exchange.Gateway.GatewayBaseClient;

public class GatewayClientResult<T>
{
    public bool IsSuccessful { get; init; }
    public T? Data { get; init; }
    public HttpStatusCode? ErrorStatusCode { get; init; }
}

