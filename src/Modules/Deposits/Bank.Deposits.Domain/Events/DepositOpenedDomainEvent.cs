namespace Bank.Deposits.Domain.Events;

/// <summary>
/// Domain event raised when a new deposit account is opened
/// </summary>
public sealed record DepositOpenedDomainEvent(
    Guid DepositId,
    Guid CustomerId,
    string AccountNumber,
    decimal InitialDeposit,
    DateTime OccurredAtUtc) : DomainEvent;
