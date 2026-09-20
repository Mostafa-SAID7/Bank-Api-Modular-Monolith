namespace Bank.Cards.Domain.Events;

/// <summary>
/// Domain event raised when card PIN is changed
/// </summary>
public sealed record CardPinChangedDomainEvent(
    Guid CardId,
    Guid CustomerId) : DomainEvent(CardId, "CardPinChanged");
