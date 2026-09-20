namespace Bank.Loans.Domain.Entities;

/// <summary>
/// Loan amortization schedule entity
/// Tracks individual payment schedules (EMIs) for a loan
/// </summary>
public class LoanSchedule
{
    public Guid Id { get; set; }
    
    /// <summary>Reference to parent Loan aggregate</summary>
    public Guid LoanId { get; set; }
    
    /// <summary>Sequential EMI number (1-based)</summary>
    public int InstallmentNumber { get; set; }
    
    /// <summary>EMI due date</summary>
    public DateTime DueDateUtc { get; set; }
    
    /// <summary>Principal component of this EMI</summary>
    public decimal PrincipalAmount { get; set; }
    
    /// <summary>Interest component of this EMI</summary>
    public decimal InterestAmount { get; set; }
    
    /// <summary>Total EMI amount (Principal + Interest)</summary>
    public decimal TotalAmount { get; set; }
    
    /// <summary>Outstanding principal after this EMI payment</summary>
    public decimal OutstandingPrincipal { get; set; }
    
    /// <summary>Whether this EMI has been paid</summary>
    public bool IsPaid { get; set; }
    
    /// <summary>Date when EMI was paid (if paid)</summary>
    public DateTime? PaidDateUtc { get; set; }
    
    /// <summary>Amount actually paid (may differ from TotalAmount)</summary>
    public decimal? PaidAmount { get; set; }
    
    /// <summary>Whether this EMI payment is overdue</summary>
    public bool IsOverdue { get; set; }
    
    /// <summary>Overdue days (if IsOverdue is true)</summary>
    public int? OverdueDays { get; set; }
    
    /// <summary>Penalty charges due to late payment (if any)</summary>
    public decimal PenaltyCharges { get; set; }
    
    /// <summary>Creation timestamp</summary>
    public DateTime CreatedAtUtc { get; set; }
    
    /// <summary>Last update timestamp</summary>
    public DateTime? UpdatedAtUtc { get; set; }
}
