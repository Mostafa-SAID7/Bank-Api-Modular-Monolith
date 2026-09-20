namespace Bank.Cards.Domain.Events;

/// <summary>
/// Domain event raised when a card is replaced
/// </summary>
public sealed record CardReplacedDomainEvent(
    Guid OldCardId,
    Guid CustomerId,
    Guid NewCardId,
    string NewMaskedPan) : DomainEvent(OldCardId, "CardReplaced");
