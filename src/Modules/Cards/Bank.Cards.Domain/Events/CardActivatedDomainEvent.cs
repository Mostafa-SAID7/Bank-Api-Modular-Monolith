namespace Bank.Cards.Domain.Events;

/// <summary>
/// Domain event raised when a card is activated
/// </summary>
public sealed record CardActivatedDomainEvent(
    Guid CardId,
    Guid CustomerId) : DomainEvent(CardId, "CardActivated");
