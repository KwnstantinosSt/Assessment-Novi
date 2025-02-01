// Copyright © 2025 Konstantinos Stougiannou

using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Currency.Exchange.Common.Microservices;

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApi(this IServiceCollection services)
    {
        return services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, _) =>
            {
                document.Info.Contact = new OpenApiContact
                {
                    Name = "Exchange Currency Manager",
                    Email = "kwnstantinosst123@gmail.com",
                };

                return Task.CompletedTask;
            });
        });
    }
}
