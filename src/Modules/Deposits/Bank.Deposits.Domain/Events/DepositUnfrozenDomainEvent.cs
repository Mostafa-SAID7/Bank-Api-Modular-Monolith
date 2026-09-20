namespace Bank.Deposits.Domain.Events;

/// <summary>
/// Domain event raised when a deposit account is unfrozen
/// </summary>
public sealed record DepositUnfrozenDomainEvent(
    Guid DepositId,
    Guid CustomerId,
    string AccountNumber,
    DateTime OccurredAtUtc) : DomainEvent;
