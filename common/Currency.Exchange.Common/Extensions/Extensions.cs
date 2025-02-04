// Copyright © 2025 Konstantinos Stougiannou

namespace Currency.Exchange.Common.Extensions;

public static class Extensions
{
    public static long GenerateRandomLong()
    {
        return BitConverter.ToInt64(value: Guid.NewGuid().ToByteArray(), 0);
    }
}
