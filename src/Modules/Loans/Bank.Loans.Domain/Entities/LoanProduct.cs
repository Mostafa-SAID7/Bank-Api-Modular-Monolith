namespace Bank.Loans.Domain.Entities;

/// <summary>
/// Loan product definition entity
/// Defines product characteristics, limits, and eligibility criteria
/// </summary>
public class LoanProduct
{
    public Guid Id { get; set; }
    
    /// <summary>Product name (e.g., "Personal Loan Gold")</summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>Product description and features</summary>
    public string? Description { get; set; }
    
    /// <summary>Loan type (Personal, Home, Auto, etc.)</summary>
    public int Type { get; set; } // LoanType enum
    
    /// <summary>Minimum loan amount in currency units</summary>
    public decimal MinimumAmount { get; set; }
    
    /// <summary>Maximum loan amount in currency units</summary>
    public decimal MaximumAmount { get; set; }
    
    /// <summary>Minimum tenure in months</summary>
    public int MinimumTenureMonths { get; set; }
    
    /// <summary>Maximum tenure in months</summary>
    public int MaximumTenureMonths { get; set; }
    
    /// <summary>Base annual interest rate (percentage)</summary>
    public decimal BaseAnnualInterestRate { get; set; }
    
    /// <summary>Interest rate type (Fixed, Floating, Hybrid)</summary>
    public int InterestType { get; set; } // InterestType enum
    
    /// <summary>EMI payment frequency</summary>
    public int EMIFrequency { get; set; } // EMIFrequency enum
    
    /// <summary>Processing fee as percentage of loan amount</summary>
    public decimal ProcessingFeePercent { get; set; }
    
    /// <summary>Prepayment charges allowed</summary>
    public bool AllowsPrepayment { get; set; }
    
    /// <summary>Prepayment penalty as percentage of outstanding principal</summary>
    public decimal? PrepaymentPenaltyPercent { get; set; }
    
    /// <summary>Whether product requires collateral</summary>
    public bool RequiresCollateral { get; set; }
    
    /// <summary>Minimum credit score required</summary>
    public int? MinimumCreditScore { get; set; }
    
    /// <summary>Maximum debt-to-income ratio allowed</summary>
    public decimal? MaximumDebtToIncomeRatio { get; set; }
    
    /// <summary>Product is currently active and can be applied for</summary>
    public bool IsActive { get; set; }
    
    /// <summary>Creation timestamp</summary>
    public DateTime CreatedAtUtc { get; set; }
    
    /// <summary>Last update timestamp</summary>
    public DateTime? UpdatedAtUtc { get; set; }
}
