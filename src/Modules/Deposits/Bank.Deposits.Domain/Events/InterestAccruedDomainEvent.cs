namespace Bank.Deposits.Domain.Events;

/// <summary>
/// Domain event raised when interest is accrued to a deposit account
/// </summary>
public sealed record InterestAccruedDomainEvent(
    Guid DepositId,
    Guid CustomerId,
    string AccountNumber,
    decimal InterestAmount,
    decimal TotalAccruedInterest,
    DateTime OccurredAtUtc) : DomainEvent(DepositId, "InterestAccrued");
