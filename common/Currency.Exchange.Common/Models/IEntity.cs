// Copyright © 2025 Konstantinos Stougiannou

namespace Currency.Exchange.Common.Models;

public interface IEntity
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
