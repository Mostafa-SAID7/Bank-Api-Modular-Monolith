namespace Bank.Deposits.Domain.Events;

/// <summary>
/// Domain event raised when a fixed deposit reaches maturity
/// </summary>
public sealed record DepositMaturedDomainEvent(
    Guid DepositId,
    Guid CustomerId,
    string AccountNumber,
    decimal FinalBalance,
    DateTime OccurredAtUtc) : DomainEvent(DepositId, "DepositMatured");
