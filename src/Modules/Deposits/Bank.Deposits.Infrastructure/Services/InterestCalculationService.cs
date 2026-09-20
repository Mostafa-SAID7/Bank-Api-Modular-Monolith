namespace Bank.Deposits.Infrastructure.Services;

/// <summary>
/// Calculates interest for deposit accounts
/// Supports both Simple and Compound interest calculation methods
/// Implements IInterestCalculationService interface from Application layer
/// </summary>
public sealed class InterestCalculationService : IInterestCalculationService
{
    private const decimal DaysInYear = 365M;

    /// <summary>
    /// Calculates simple or compound interest based on the specified method
    /// </summary>
    /// <param name="principal">Principal amount</param>
    /// <param name="annualRate">Annual interest rate (as percentage, e.g., 5 for 5%)</param>
    /// <param name="daysInPeriod">Number of days in the calculation period</param>
    /// <param name="daysInYear">Days in year (typically 365 or 360)</param>
    /// <param name="method">Interest calculation method (Simple or Compound)</param>
    /// <returns>Calculated interest amount</returns>
    public decimal CalculateInterest(
        decimal principal,
        decimal annualRate,
        int daysInPeriod,
        int daysInYear,
        InterestCalculationMethod method)
    {
        if (principal <= 0 || annualRate < 0 || daysInPeriod <= 0 || daysInYear <= 0)
        {
            return 0M;
        }

        return method == InterestCalculationMethod.Simple
            ? CalculateSimpleInterest(principal, annualRate, daysInPeriod, daysInYear)
            : CalculateCompoundInterest(principal, annualRate, daysInPeriod, daysInYear);
    }

    /// <summary>
    /// Calculates the maturity amount for a fixed deposit
    /// Maturity Amount = Principal + Interest
    /// </summary>
    /// <param name="principal">Principal amount</param>
    /// <param name="annualRate">Annual interest rate (as percentage)</param>
    /// <param name="termMonths">Term duration in months</param>
    /// <param name="method">Interest calculation method (Simple or Compound)</param>
    /// <returns>Maturity amount (principal + interest)</returns>
    public decimal CalculateMaturityAmount(
        decimal principal,
        decimal annualRate,
        int termMonths,
        InterestCalculationMethod method)
    {
        if (principal <= 0 || annualRate < 0 || termMonths <= 0)
        {
            return principal;
        }

        // Convert months to days (using 30 days per month for uniformity)
        int days = termMonths * 30;

        decimal interest = method == InterestCalculationMethod.Simple
            ? CalculateSimpleInterest(principal, annualRate, days, 360)
            : CalculateCompoundInterest(principal, annualRate, days, 360);

        return principal + interest;
    }

    /// <summary>
    /// Calculates compound interest over multiple periods
    /// Formula: A = P * (1 + r/n)^(n*t)
    /// where: P = principal, r = annual rate, n = compounding frequency, t = time in years
    /// </summary>
    /// <param name="principal">Principal amount</param>
    /// <param name="annualRate">Annual interest rate (as percentage)</param>
    /// <param name="monthsElapsed">Number of months elapsed</param>
    /// <param name="compoundingFrequency">Compounding frequency in months (1=monthly, 3=quarterly, 12=annually)</param>
    /// <returns>Calculated compound interest</returns>
    public decimal CalculateCompoundInterest(
        decimal principal,
        decimal annualRate,
        int monthsElapsed,
        int compoundingFrequency)
    {
        if (principal <= 0 || annualRate < 0 || monthsElapsed <= 0 || compoundingFrequency <= 0)
        {
            return 0M;
        }

        // Convert percentage to decimal (5% = 0.05)
        decimal rateDecimal = annualRate / 100M;
        
        // Calculate number of compounding periods
        decimal periods = (decimal)monthsElapsed / compoundingFrequency;
        
        // Convert to years for annual rate
        decimal years = (decimal)monthsElapsed / 12M;
        
        // Compound interest formula: A = P(1 + r)^t
        // For compounding frequency: A = P(1 + r/n)^(n*t)
        decimal rate = rateDecimal / (12M / compoundingFrequency); // Adjust rate for compounding frequency
        decimal maturityAmount = principal * (decimal)Math.Pow((double)(1 + rate / 12M), (double)monthsElapsed);
        
        return maturityAmount - principal;
    }

    /// <summary>
    /// Calculates simple interest
    /// Formula: Interest = Principal * Rate * Time
    /// where: Time is expressed as a fraction of the year
    /// </summary>
    private static decimal CalculateSimpleInterest(
        decimal principal,
        decimal annualRate,
        int daysInPeriod,
        int daysInYear)
    {
        // Convert percentage to decimal (5% = 0.05)
        decimal rateDecimal = annualRate / 100M;
        
        // Calculate interest: P * R * T
        // T = days / daysInYear
        decimal interest = principal * rateDecimal * (daysInPeriod / (decimal)daysInYear);
        
        return interest;
    }
}
