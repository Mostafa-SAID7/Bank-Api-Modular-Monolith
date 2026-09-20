namespace Bank.Loans.Application.Interfaces;

/// <summary>
/// Service for calculating EMI (Equated Monthly Installment)
/// Supports multiple calculation methods and loan types
/// </summary>
public interface IEMICalculationService
{
    /// <summary>
    /// Calculate monthly EMI using standard formula
    /// EMI = (P * R * (1 + R)^N) / ((1 + R)^N - 1)
    /// where P = Principal, R = Monthly Rate, N = Number of months
    /// </summary>
    decimal CalculateMonthlyEMI(decimal principal, decimal annualRate, int tenureMonths);
    
    /// <summary>
    /// Generate complete amortization schedule for the loan
    /// Returns list of LoanSchedule entities for all EMIs
    /// </summary>
    List<LoanSchedule> GenerateAmortizationSchedule(
        Guid loanId,
        decimal principal,
        decimal annualRate,
        int tenureMonths,
        int emiFrequency);
    
    /// <summary>
    /// Calculate total interest payable over loan tenure
    /// </summary>
    decimal CalculateTotalInterest(decimal principal, decimal annualRate, int tenureMonths);
    
    /// <summary>
    /// Calculate remaining principal and interest components
    /// </summary>
    (decimal PrincipalComponent, decimal InterestComponent) CalculateEMIComponents(
        decimal remainingPrincipal,
        decimal annualRate,
        int remainingMonths);
}
