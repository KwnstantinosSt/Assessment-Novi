// Copyright © 2025 Konstantinos Stougiannou

using Currency.Exchange.Common.ValidationFactory;
using FluentValidation;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Currency.Exchange;

public static class DependencyInjection
{
    public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // Register fluentValidation with custom result factory to format api responses
        services.AddFluentValidationAutoValidation(conf =>
        {
            conf.OverrideDefaultResultFactoryWith<ValidationResultFactory>();
        });

        // Register fluent validators
        // services.AddValidatorsFromAssemblyContaining<UserTokenValidator>();

        // Resister services
    }
}
