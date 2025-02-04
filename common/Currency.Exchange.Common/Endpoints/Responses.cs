// Copyright © 2025 Konstantinos Stougiannou

using System.ComponentModel;

namespace Currency.Exchange.Common.Endpoints;

public record SuccessfulResponse(string Code = "200", string Message = "All good")
{
    [Description(description: "Always \"200\" on success")]
    public string Code { get; } = Code;

    [Description(description: "Informative description of the result")]
    public string Message { get; } = Message;
}

public record SuccessfulResponseWithData<T>(T? Data, string Code = "200", string Message = "All good")
{
    [Description(description: "Always \"200\" on success")]
    public string Code { get; } = Code;

    [Description(description: "Informative description of the result")]
    public string Message { get; } = Message;

    [Description(description: "Generic result data")]
    public T? Data { get; } = Data;
}

public record ErrorResponse
{
    [Description(description: "The error code returned.")]
    public required string Code { get; init; }

    [Description(description: "Informative description of the result")]
    public required string Message { get; init; }
}

public record ErrorResponseWithDetails<T>
{
    [Description(description: "The error code returned.")]
    public required string Code { get; init; }

    [Description(description: "Informative description of the result")]
    public required string Message { get; init; }

    [Description(description: "Additional details about the error")]
    public required T Details { get; init; }
}
