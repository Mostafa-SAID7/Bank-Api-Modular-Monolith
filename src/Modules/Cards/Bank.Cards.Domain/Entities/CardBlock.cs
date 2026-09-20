namespace Bank.Cards.Domain.Entities;

/// <summary>
/// Tracks card block history (lost, stolen, fraud, etc.)
/// </summary>
public class CardBlock
{
    public Guid Id { get; private set; }
    public Guid CardId { get; private set; }
    public BlockReason Reason { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public bool Permanent { get; private set; }
    
    public DateTime BlockedAtUtc { get; private set; }
    public DateTime? UnblockedAtUtc { get; private set; }
    
    public string? BlockedByUserId { get; private set; }
    public string? UnblockedByUserId { get; private set; }

    /// <summary>
    /// Create a new card block record
    /// </summary>
    public static CardBlock Create(
        Guid cardId,
        BlockReason reason,
        string description = "",
        bool permanent = false)
    {
        return new CardBlock
        {
            Id = Guid.NewGuid(),
            CardId = cardId,
            Reason = reason,
            Description = description,
            Permanent = permanent,
            BlockedAtUtc = DateTime.UtcNow,
        };
    }

    /// <summary>
    /// Unblock the card
    /// </summary>
    public void Unblock()
    {
        if (Permanent)
            throw new InvalidOperationException("Cannot unblock a card with permanent block");

        UnblockedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Check if block is currently active
    /// </summary>
    public bool IsActive() => UnblockedAtUtc == null;
}
