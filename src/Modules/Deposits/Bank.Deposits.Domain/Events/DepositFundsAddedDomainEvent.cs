namespace Bank.Deposits.Domain.Events;

/// <summary>
/// Domain event raised when funds are added to a deposit account
/// </summary>
public sealed record DepositFundsAddedDomainEvent(
    Guid DepositId,
    Guid CustomerId,
    string AccountNumber,
    decimal Amount,
    decimal NewBalance,
    DateTime OccurredAtUtc) : DomainEvent(DepositId, "DepositFundsAdded");
