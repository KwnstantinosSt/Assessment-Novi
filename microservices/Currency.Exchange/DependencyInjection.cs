// Copyright © 2025 Konstantinos Stougiannou

using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Currency.Exchange;

public static class DependencyInjection
{
    public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFluentValidationAutoValidation(conf =>
        {
            // conf.OverrideDefaultResultFactoryWith<ValidationResultFactory>();
        });
    }
}
