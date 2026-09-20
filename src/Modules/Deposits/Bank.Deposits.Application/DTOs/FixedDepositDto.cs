namespace Bank.Deposits.Application.DTOs;

/// <summary>
/// DTO for fixed deposit details
/// </summary>
public record FixedDepositDto(
    Guid Id,
    string FixedDepositNumber,
    Guid CustomerId,
    Guid DepositProductId,
    decimal PrincipalAmount,
    FixedDepositStatus Status,
    decimal CurrentRate,
    int TermMonths,
    DateTime StartDate,
    DateTime MaturityDate,
    decimal AccruedInterest,
    InterestFrequency InterestFrequency,
    InterestCalculationMethod InterestCalculationMethod,
    bool AutoRenewalEnabled,
    decimal WithdrawalPenaltyPercent,
    DateTime CreatedAtUtc,
    DateTime LastModifiedAtUtc);
