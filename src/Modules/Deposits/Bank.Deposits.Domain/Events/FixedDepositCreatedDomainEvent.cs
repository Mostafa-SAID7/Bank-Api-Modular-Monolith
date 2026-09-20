namespace Bank.Deposits.Domain.Events;

/// <summary>
/// Domain event raised when a fixed deposit is created
/// </summary>
public sealed record FixedDepositCreatedDomainEvent(
    Guid FixedDepositId,
    Guid CustomerId,
    string DepositNumber,
    decimal PrincipalAmount,
    decimal InterestRate,
    int TermDays,
    DateTime MaturityDateUtc,
    DateTime OccurredAtUtc) : DomainEvent(FixedDepositId, "FixedDepositCreated");
