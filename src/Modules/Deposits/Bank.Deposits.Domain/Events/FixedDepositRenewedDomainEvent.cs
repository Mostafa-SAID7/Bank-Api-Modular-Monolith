namespace Bank.Deposits.Domain.Events;

/// <summary>
/// Domain event raised when a fixed deposit is renewed
/// </summary>
public sealed record FixedDepositRenewedDomainEvent(
    Guid OriginalDepositId,
    Guid NewDepositId,
    Guid CustomerId,
    string NewDepositNumber,
    decimal RenewalAmount,
    int NewTermDays,
    DateTime NewMaturityDateUtc,
    DateTime OccurredAtUtc) : DomainEvent(NewDepositId, "FixedDepositRenewed");
