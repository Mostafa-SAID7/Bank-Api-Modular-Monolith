namespace Bank.Cards.Domain.Events;

/// <summary>
/// Domain event raised when a card is closed
/// </summary>
public sealed record CardClosedDomainEvent(
    Guid CardId,
    Guid CustomerId,
    string Reason) : DomainEvent(CardId, "CardClosed");
