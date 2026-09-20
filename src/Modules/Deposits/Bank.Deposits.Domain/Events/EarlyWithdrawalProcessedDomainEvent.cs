namespace Bank.Deposits.Domain.Events;

/// <summary>
/// Domain event raised when an early withdrawal is processed on a fixed deposit
/// </summary>
public sealed record EarlyWithdrawalProcessedDomainEvent(
    Guid FixedDepositId,
    Guid CustomerId,
    string DepositNumber,
    decimal WithdrawalAmount,
    decimal PenaltyAmount,
    decimal NetAmountPaid,
    string Reason,
    DateTime OccurredAtUtc) : DomainEvent(FixedDepositId, "EarlyWithdrawalProcessed");
