namespace Bank.Loans.Domain.Enums;

/// <summary>
/// Loan lifecycle status enumeration
/// </summary>
public enum LoanStatus
{
    /// <summary>Application submitted, pending approval</summary>
    Applied = 0,
    
    /// <summary>Application approved, waiting for disbursement</summary>
    Approved = 1,
    
    /// <summary>Loan amount disbursed to customer</summary>
    Disbursed = 2,
    
    /// <summary>Active loan with ongoing payments</summary>
    Active = 3,
    
    /// <summary>Loan has been fully repaid</summary>
    Closed = 4,
    
    /// <summary>Loan was rejected by approval process</summary>
    Rejected = 5,
    
    /// <summary>Loan was cancelled before disbursement</summary>
    Cancelled = 6,
    
    /// <summary>Loan is in default (missed payments)</summary>
    Defaulted = 7
}
