namespace Bank.Deposits.Domain.Events;

/// <summary>
/// Domain event raised when a withdrawal is made from a deposit account
/// </summary>
public sealed record WithdrawalMadeDomainEvent(
    Guid DepositId,
    Guid CustomerId,
    string AccountNumber,
    decimal Amount,
    decimal NewBalance,
    string Reason,
    DateTime OccurredAtUtc) : DomainEvent;
