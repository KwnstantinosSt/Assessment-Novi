// Copyright © 2025 Konstantinos Stougiannou

namespace Currency.Exchange.Common.Models;

public interface IEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
