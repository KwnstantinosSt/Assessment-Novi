// Copyright © 2025 Konstantinos Stougiannou

using Currency.Exchange.Common.Endpoints;
using Microsoft.AspNetCore.Http;

namespace Currency.Exchange.Common.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(value: new ErrorResponse
            {
                Message = ex.Message,
                Code = "400",
            });
        }
    }

}
