namespace Bank.Cards.Domain.Events;

/// <summary>
/// Domain event raised when card limits are updated
/// </summary>
public sealed record CardLimitsUpdatedDomainEvent(
    Guid CardId,
    Guid CustomerId,
    decimal DailyLimit,
    decimal TransactionLimit) : DomainEvent(CardId, "CardLimitsUpdated");
