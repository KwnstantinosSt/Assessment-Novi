// Copyright © 2025 Konstantinos Stougiannou

namespace Currency.Exchange.Common.Dto;

public class ErrorResponse
{
    public ErrorResponse()
    {
    }

    public ErrorResponse(bool isSuccessful, string? message)
    {
        IsSuccessful = isSuccessful;
        Message = message;
    }

    public bool IsSuccessful { get; set; }
    public string? Message { get; set; }
}
