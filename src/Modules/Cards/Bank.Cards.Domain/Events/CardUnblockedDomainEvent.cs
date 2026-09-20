namespace Bank.Cards.Domain.Events;

/// <summary>
/// Domain event raised when a blocked card is unblocked
/// </summary>
public sealed record CardUnblockedDomainEvent(
    Guid CardId,
    Guid CustomerId) : DomainEvent(CardId, "CardUnblocked");
