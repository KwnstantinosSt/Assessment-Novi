// Copyright © 2025 Konstantinos Stougiannou

using Currency.Exchange.Common.Endpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Currency.Exchange.Features.Wallets.CreateWallet;

public class CreateWalletEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(pattern: "api/wallets", CreateWallet)
            .WithDescription("Creates a new wallet")
            .WithSummary(summary: "Creates a wallet")
            .WithName(endpointName: "CreateWallet")
            .WithTags("Wallets")
            .AddFluentValidationAutoValidation();
    }

    public async Task<Results<Ok<CreateWalletResponse>, BadRequest<ErrorResponse>>> CreateWallet(
        [FromServices] CreateWalletHandler handler)
    {
        var (handlerResponse, errorResponse) = await handler.Handle();

        if (!errorResponse.IsSuccessful)
        {
            return TypedResults.Extensions.BadRequest(message: errorResponse.Message!);
        }

        return TypedResults.Extensions.Success(response: handlerResponse);
    }
}
