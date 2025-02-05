// Copyright © 2025 Konstantinos Stougiannou

using Currency.Exchange.Common.Endpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Currency.Exchange.Features.Wallets.EditWalletBalance;

public class EditWalletBalanceEndpoint : IEndpoint
{
    private readonly List<string> _strategies = new() { "AddFundsStrategy", "SubtractFundsStrategy", "ForceSubstractFundsStrategy" };
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/wallets/ajustbalance/{walletId}", EditWallet)
            .WithDescription("Edit a wallet with {id} and converts to a specific {currency} and adding or substructing the balance")
            .WithSummary(summary: "Edit a wallet balace to any currency and amount")
            .WithName(endpointName: "EditWalletBalance")
            .WithTags("Wallets")
            .AddFluentValidationAutoValidation();
    }

    public async Task<Results<Ok<EditWalletBalanceResponse>, BadRequest<ErrorResponse>>> EditWallet(
        [FromRoute] int walletId,
        [FromQuery] decimal amount,
        [FromQuery] string strategy,
        [FromServices] EditWalletBalanceHandler handler,
        [FromQuery] string? currency = "EUR")
    {
        if (walletId == 0)
            return TypedResults.Extensions.BadRequest("Wallet id is required");

        if (amount <= 0)
            return TypedResults.Extensions.BadRequest("Amount must be greater than 0");

        if (!(_strategies.Any(s => s.ToUpper() == strategy.ToUpper())))
        {
            return TypedResults.Extensions.BadRequest("Strategy does not exists");
        }

        if (string.IsNullOrEmpty(currency))
            return TypedResults.Extensions.BadRequest(message: "Currency is required");

        var (handlerResponse, errorResponse) = await handler.Handle(walletId, currency, strategy, amount);

        if (!errorResponse.IsSuccessful)
        {
            return TypedResults.Extensions.BadRequest(message: errorResponse.Message!);
        }

        return TypedResults.Extensions.Success(response: handlerResponse);
    }
}
