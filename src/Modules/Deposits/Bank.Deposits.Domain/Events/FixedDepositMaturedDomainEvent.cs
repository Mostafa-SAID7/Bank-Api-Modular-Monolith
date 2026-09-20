namespace Bank.Deposits.Domain.Events;

/// <summary>
/// Domain event raised when a fixed deposit reaches maturity
/// </summary>
public sealed record FixedDepositMaturedDomainEvent(
    Guid FixedDepositId,
    Guid CustomerId,
    string DepositNumber,
    decimal PrincipalAmount,
    decimal AccruedInterest,
    decimal MaturityAmount,
    DateTime MaturityDateUtc,
    DateTime OccurredAtUtc) : DomainEvent(FixedDepositId, "FixedDepositMatured");
