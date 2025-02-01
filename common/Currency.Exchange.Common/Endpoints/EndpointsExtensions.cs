// Copyright © 2025 Konstantinos Stougiannou

using Microsoft.AspNetCore.Builder;

namespace Currency.Exchange.Common.Endpoints;

public static class EndpointsExtensions
{
    public static void MapMinimalEndpoints(this WebApplication app)
    {
        var endpointType = typeof(IEndpoint);

        var minimalApis = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(c => endpointType.IsAssignableFrom(c) && c.IsClass)
            .ToArray();

        foreach (var minimalApi in minimalApis)
        {
            var minimalApiInstance = Activator.CreateInstance(minimalApi) as IEndpoint;
            minimalApiInstance!.MapEndpoint(app);
        }
    }
}
