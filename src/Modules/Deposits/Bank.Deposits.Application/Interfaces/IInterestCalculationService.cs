namespace Bank.Deposits.Application.Interfaces;

/// <summary>
/// Service interface for calculating deposit interest
/// </summary>
public interface IInterestCalculationService
{
    /// <summary>
    /// Calculate interest for a period
    /// </summary>
    decimal CalculateInterest(
        decimal principal,
        decimal annualRate,
        int daysInPeriod,
        int daysInYear,
        InterestCalculationMethod method);

    /// <summary>
    /// Calculate maturity amount for fixed deposit
    /// </summary>
    decimal CalculateMaturityAmount(
        decimal principal,
        decimal annualRate,
        int monthsTerm,
        InterestCalculationMethod method);

    /// <summary>
    /// Calculate interest with compounding
    /// </summary>
    decimal CalculateCompoundInterest(
        decimal principal,
        decimal annualRate,
        int monthsElapsed,
        InterestFrequency compoundingFrequency);
}
