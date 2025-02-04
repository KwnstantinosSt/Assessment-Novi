// Copyright © 2025 Konstantinos Stougiannou

using Currency.Exchange.BackgroundServices;
using Currency.Exchange.Common.ValidationFactory;
using Currency.Exchange.Features.Wallets.CreateWallet;
using Currency.Exchange.Features.Wallets.EditWalletBalance;
using Currency.Exchange.Features.Wallets.GetWalletBalance;
using Currency.Exchange.Gateway.Configuration;
using Currency.Exchange.Gateway.EuropeanCentralBankClient;
using Currency.Exchange.Gateway.GatewayBaseClient;
using FluentValidation;
using Quartz;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Currency.Exchange;

public static class DependencyInjection
{
    public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // Register gateway client and europeanCentralBankClient
        var client = services.AddGatewayClient();
        client.Services.AddTransient<IEuropeanCentralBankClient, EuropeanCentralBankClient>();
        // register configuration of europeanCentralBankClient
        client.Services.Configure<GatewayClientConfiguration>(config: configuration.GetSection(key: "GatewayClient"));

        // Register fluentValidation with custom result factory to format api responses
        services.AddFluentValidationAutoValidation(conf =>
        {
            conf.OverrideDefaultResultFactoryWith<ValidationResultFactory>();
        });

        // Register fluent validators
        services.AddValidatorsFromAssemblyContaining<CreateWalletValidator>();
        services.AddValidatorsFromAssemblyContaining<EditWalletBalanceValidator>();
        services.AddValidatorsFromAssemblyContaining<GetWalletBalanceValidator>();

        // Register quartz and background service
        services.AddQuartz(opt =>
        {
            var jobKey = JobKey.Create(name: nameof(GatewayBackgroundService));
            opt.AddJob<GatewayBackgroundService>(jobKey)
                .AddTrigger(trigger =>
                    trigger.ForJob(jobKey)
                        .WithSimpleSchedule(schedule =>
                            schedule.WithIntervalInMinutes(minutes:
                                    Convert.ToInt32(value:
                                        configuration[key: "BackgroundJobs:GatewayBackgroundJobIntervalInMinutes"]))
                                .RepeatForever()));
        });

        services.AddQuartzHostedService();
    }
}
