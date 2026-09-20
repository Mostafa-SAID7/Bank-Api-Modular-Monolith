namespace Bank.Deposits.Domain.Events;

/// <summary>
/// Domain event raised when a deposit account is frozen
/// </summary>
public sealed record DepositFrozenDomainEvent(
    Guid DepositId,
    Guid CustomerId,
    string AccountNumber,
    string Reason,
    DateTime OccurredAtUtc) : DomainEvent(DepositId, "DepositFrozen");
