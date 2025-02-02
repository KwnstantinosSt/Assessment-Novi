// Copyright © 2025 Konstantinos Stougiannou

using Currency.Exchange.Common.Endpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Results;
using SharpGrip.FluentValidation.AutoValidation.Shared.Extensions;

namespace Currency.Exchange.Common.ValidationFactory;

public class ValidationResultFactory : IFluentValidationAutoValidationResultFactory
{
    public IResult CreateResult(EndpointFilterInvocationContext context, ValidationResult validationResult)
    {
        var validationProblemErrors = validationResult.ToValidationProblemErrors();

        return Results.Extensions.BadRequest(validationProblemErrors);
    }
}
