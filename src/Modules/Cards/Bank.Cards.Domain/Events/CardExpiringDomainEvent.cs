namespace Bank.Cards.Domain.Events;

/// <summary>
/// Domain event raised when a card is expiring soon (within 30 days)
/// </summary>
public sealed record CardExpiringDomainEvent(
    Guid CardId,
    Guid CustomerId,
    DateTime ExpiryDateUtc) : DomainEvent(CardId, "CardExpiring");
