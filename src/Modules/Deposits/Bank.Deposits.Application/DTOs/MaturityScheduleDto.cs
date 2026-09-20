namespace Bank.Deposits.Application.DTOs;

/// <summary>
/// DTO for maturity schedule details
/// </summary>
public record MaturityScheduleDto(
    Guid FixedDepositId,
    string FixedDepositNumber,
    decimal PrincipalAmount,
    decimal CurrentRate,
    int TermMonths,
    DateTime StartDate,
    DateTime MaturityDate,
    int RemainingDays,
    decimal AccruedInterest,
    decimal MaturityAmount,
    bool AutoRenewalEnabled,
    FixedDepositStatus Status);
