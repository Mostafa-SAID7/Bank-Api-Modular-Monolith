namespace Bank.Loans.Infrastructure.Services;

/// <summary>
/// Service for calculating EMI (Equated Monthly Installment) and amortization schedules
/// Supports fixed and floating rate calculations
/// Uses standard EMI formula: EMI = (P × r × (1 + r)^n) / ((1 + r)^n - 1)
/// where P = Principal, r = Monthly interest rate, n = Number of months
/// </summary>
public sealed class EMICalculationService : IEMICalculationService
{
    /// <summary>
    /// Calculates monthly EMI based on principal, tenure, and interest rate
    /// </summary>
    public decimal CalculateMonthlyEMI(decimal principal, decimal annualInterestRate, int tenureMonths)
    {
        if (principal <= 0)
            throw new ArgumentException("Principal must be greater than 0", nameof(principal));
        
        if (annualInterestRate < 0)
            throw new ArgumentException("Interest rate cannot be negative", nameof(annualInterestRate));
        
        if (tenureMonths <= 0)
            throw new ArgumentException("Tenure must be greater than 0", nameof(tenureMonths));

        // If interest rate is 0, EMI is simply principal divided by number of months
        if (annualInterestRate == 0)
        {
            return Math.Round(principal / tenureMonths, 2);
        }

        // Convert annual rate to monthly rate (as decimal)
        var monthlyRate = annualInterestRate / 100 / 12;

        // EMI = (P × r × (1 + r)^n) / ((1 + r)^n - 1)
        var numerator = principal * monthlyRate * (decimal)Math.Pow((double)(1 + monthlyRate), tenureMonths);
        var denominator = (decimal)Math.Pow((double)(1 + monthlyRate), tenureMonths) - 1;

        var emi = numerator / denominator;
        return Math.Round(emi, 2);
    }

    /// <summary>
    /// Generates complete amortization schedule for the loan
    /// Returns list of LoanSchedule entities for all EMIs
    /// </summary>
    public List<LoanSchedule> GenerateAmortizationSchedule(
        Guid loanId,
        decimal principal,
        decimal annualInterestRate,
        int tenureMonths,
        int emiFrequency)
    {
        var schedule = new List<LoanSchedule>();
        
        var emi = CalculateMonthlyEMI(principal, annualInterestRate, tenureMonths);
        var monthlyRate = annualInterestRate == 0 ? 0 : annualInterestRate / 100 / 12;
        var remainingBalance = principal;

        for (int month = 1; month <= tenureMonths; month++)
        {
            // Interest for this month = Remaining balance × monthly rate
            var interestPayment = Math.Round(remainingBalance * monthlyRate, 2);
            
            // Principal payment = EMI - Interest
            var principalPayment = Math.Round(emi - interestPayment, 2);
            
            // For the last month, adjust to ensure exact payoff
            if (month == tenureMonths)
            {
                principalPayment = remainingBalance;
            }

            remainingBalance = Math.Max(0, Math.Round(remainingBalance - principalPayment, 2));

            var scheduleEntry = new LoanSchedule
            {
                Id = Guid.NewGuid(),
                LoanId = loanId,
                InstallmentNumber = month,
                DueDateUtc = DateTime.UtcNow.AddMonths(month),
                PrincipalAmount = principalPayment,
                InterestAmount = interestPayment,
                TotalAmount = emi,
                OutstandingPrincipal = remainingBalance
            };

            schedule.Add(scheduleEntry);
        }

        return schedule;
    }

    /// <summary>
    /// Calculates total interest payable over the loan tenure
    /// </summary>
    public decimal CalculateTotalInterest(decimal principal, decimal annualInterestRate, int tenureMonths)
    {
        if (annualInterestRate == 0)
            return 0;

        var emi = CalculateMonthlyEMI(principal, annualInterestRate, tenureMonths);
        var totalAmount = emi * tenureMonths;
        var totalInterest = totalAmount - principal;

        return Math.Round(totalInterest, 2);
    }

    /// <summary>
    /// Calculates EMI components (principal and interest portions)
    /// for a specific remaining period
    /// </summary>
    public (decimal PrincipalComponent, decimal InterestComponent) CalculateEMIComponents(
        decimal remainingPrincipal,
        decimal annualInterestRate,
        int remainingMonths)
    {
        if (remainingMonths <= 0)
            throw new ArgumentException("Remaining months must be greater than 0", nameof(remainingMonths));

        var monthlyRate = annualInterestRate == 0 ? 0 : annualInterestRate / 100 / 12;
        var emi = CalculateMonthlyEMI(remainingPrincipal, annualInterestRate, remainingMonths);
        var interestComponent = Math.Round(remainingPrincipal * monthlyRate, 2);
        var principalComponent = Math.Round(emi - interestComponent, 2);

        return (principalComponent, interestComponent);
    }
}

