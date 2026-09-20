namespace Bank.Loans.Domain.Enums;

/// <summary>
/// Loan product type enumeration
/// </summary>
public enum LoanType
{
    /// <summary>Personal unsecured loan</summary>
    Personal = 0,
    
    /// <summary>Home/mortgage loan</summary>
    Home = 1,
    
    /// <summary>Auto/vehicle loan</summary>
    Auto = 2,
    
    /// <summary>Education loan</summary>
    Education = 3,
    
    /// <summary>Business loan</summary>
    Business = 4,
    
    /// <summary>Line of credit</summary>
    LineOfCredit = 5,
    
    /// <summary>Gold loan (collateral-based)</summary>
    Gold = 6,
    
    /// <summary>Agriculture loan</summary>
    Agriculture = 7
}
