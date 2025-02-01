// Copyright © 2025 Konstantinos Stougiannou

using Microsoft.AspNetCore.Routing;

namespace Currency.Exchange.Common.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder builder);
}
