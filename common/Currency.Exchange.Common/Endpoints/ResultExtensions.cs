// Copyright © 2025 Konstantinos Stougiannou

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Serilog;

namespace Currency.Exchange.Common.Endpoints;

public static class ResultExtensions
{
    public static Ok<SuccessfulResponse> Success(this IResultExtensions resultExtensions) =>
        TypedResults.Ok(value: new SuccessfulResponse());

    public static Ok<SuccessfulResponse> Success(this IResultExtensions resultExtensions, string message) =>
        TypedResults.Ok(value: new SuccessfulResponse(message));

    public static Ok<T> Success<T>(this IResultExtensions resultExtensions, T response) where T : SuccessfulResponse
    {
        return TypedResults.Ok(response);
    }

    public static BadRequest<ErrorResponse> BadRequest(this IResultExtensions resultExtensions, string message = "Bad request")
    {
        LogErrorAndReturn(code: "400", message);

        return TypedResults.BadRequest(error: new ErrorResponse
        {
            Code = "400",
            Message = message,
        });
    }

    public static BadRequest<ErrorResponseWithDetails<T>> BadRequest<T>(this IResultExtensions resultExtensions, T details, string message = "Validation Failed")
    {
        LogErrorAndReturn(code: "400", message);

        return TypedResults.BadRequest(error: new ErrorResponseWithDetails<T>
        {
            Code = "400",
            Message = message,
            Details = details,
        });
    }

    public static NotFound<ErrorResponse> NotFound(this IResultExtensions resultExtensions, string message = "Not found")
    {
        LogErrorAndReturn(code: "404", message);

        return TypedResults.NotFound(value: new ErrorResponse
        {
            Code = "404",
            Message = message,
        });
    }

    public static UnprocessableEntity<ErrorResponse> ToBusinessError<T>(this T code, string description)
        where T : struct, Enum
    {
        var codeName = Enum.GetName(code)!;

        LogErrorAndReturn(codeName, description);

        return TypedResults.UnprocessableEntity(error: new ErrorResponse
        {
            Code = codeName,
            Message = description,
        });
    }

    private static void LogErrorAndReturn(string code, string message)
    {
        Log.Warning(messageTemplate: "Return error with {Code} and {Message}", code, message);
    }
}
