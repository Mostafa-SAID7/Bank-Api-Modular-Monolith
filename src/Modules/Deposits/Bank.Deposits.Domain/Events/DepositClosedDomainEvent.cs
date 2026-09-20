namespace Bank.Deposits.Domain.Events;

/// <summary>
/// Domain event raised when a deposit account is closed
/// </summary>
public sealed record DepositClosedDomainEvent(
    Guid DepositId,
    Guid CustomerId,
    string AccountNumber,
    decimal FinalBalance,
    decimal TotalInterestEarned,
    string Reason,
    DateTime OccurredAtUtc) : DomainEvent(DepositId, "DepositClosed");
