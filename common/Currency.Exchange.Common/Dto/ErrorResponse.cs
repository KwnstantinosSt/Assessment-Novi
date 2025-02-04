// Copyright © 2025 Konstantinos Stougiannou

namespace Currency.Exchange.Common.Dto;

public class ErrorResponse
{
    public bool IsSuccessful { get; set; }
    public string? Message { get; set; }
}
