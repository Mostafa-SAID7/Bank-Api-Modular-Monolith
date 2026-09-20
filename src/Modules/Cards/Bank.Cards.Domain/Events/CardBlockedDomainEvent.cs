namespace Bank.Cards.Domain.Events;

/// <summary>
/// Domain event raised when a card is blocked
/// </summary>
public sealed record CardBlockedDomainEvent(
    Guid CardId,
    Guid CustomerId,
    BlockReason Reason,
    string Description) : DomainEvent(CardId, "CardBlocked");
