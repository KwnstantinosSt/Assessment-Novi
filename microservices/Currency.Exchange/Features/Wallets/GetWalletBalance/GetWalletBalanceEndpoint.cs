// Copyright © 2025 Konstantinos Stougiannou

using Currency.Exchange.Common.Endpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using ErrorResponse = Currency.Exchange.Common.Endpoints.ErrorResponse;

namespace Currency.Exchange.Features.Wallets.GetWalletBalance;

public class GetWalletBalanceEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/wallets/{walletId}", GetWallet)
            .WithDescription("Gets a wallet with {id} and converts to a specific {currency} returning the balance")
            .WithSummary(summary: "Gets a wallet balace to any currency")
            .WithName(endpointName: "GetWalletBalance")
            .WithTags("Wallets")
            .AddFluentValidationAutoValidation();
    }

    public async Task<Results<Ok<GetWalletBalanceResponse>, BadRequest<ErrorResponse>>> GetWallet(
        [FromRoute] int walletId,
        [FromServices] GetWalletBalanceHandler handler,
        [FromQuery] string? currency = "EUR")
    {
        if (walletId == 0)
            return TypedResults.Extensions.BadRequest("Wallet id is required");

        if (string.IsNullOrEmpty(currency))
            return TypedResults.Extensions.BadRequest(message: "Currency is required");

        var (handlerResponse, errorResponse) = await handler.Handle(walletId, currency);

        if (!errorResponse.IsSuccessful)
        {
            return TypedResults.Extensions.BadRequest(message: errorResponse.Message!);
        }

        return TypedResults.Extensions.Success(response: handlerResponse);
    }
}
