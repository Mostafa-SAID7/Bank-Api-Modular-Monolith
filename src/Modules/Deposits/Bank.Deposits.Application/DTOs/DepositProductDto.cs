namespace Bank.Deposits.Application.DTOs;

/// <summary>
/// DTO for deposit product details
/// </summary>
public record DepositProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal MinimumAmount,
    decimal MaximumAmount,
    int MinimumTermMonths,
    int MaximumTermMonths,
    decimal BaseInterestRate,
    InterestFrequency InterestFrequency,
    InterestCalculationMethod InterestCalculationMethod,
    bool AllowsEarlyWithdrawal,
    WithdrawalPenaltyType WithdrawalPenaltyType,
    decimal WithdrawalPenaltyPercent,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime LastModifiedAtUtc);
