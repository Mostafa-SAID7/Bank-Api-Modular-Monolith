namespace Bank.Cards.Domain.Events;

/// <summary>
/// Domain event raised when a card is issued
/// </summary>
public sealed record CardIssuedDomainEvent(
    Guid CardId,
    Guid CustomerId,
    string MaskedPan,
    string HolderName,
    string FullCardNumber) : DomainEvent(CardId, "CardIssued");
