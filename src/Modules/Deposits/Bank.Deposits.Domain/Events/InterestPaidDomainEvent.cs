namespace Bank.Deposits.Domain.Events;

/// <summary>
/// Domain event raised when accrued interest is paid to a deposit account
/// </summary>
public sealed record InterestPaidDomainEvent(
    Guid DepositId,
    Guid CustomerId,
    string AccountNumber,
    decimal InterestAmount,
    decimal NewBalance,
    DateTime OccurredAtUtc) : DomainEvent(DepositId, "InterestPaid");
