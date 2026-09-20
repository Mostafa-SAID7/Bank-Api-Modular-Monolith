namespace Bank.Loans.Domain.Enums;

/// <summary>
/// Loan interest rate type enumeration
/// </summary>
public enum InterestType
{
    /// <summary>Interest rate is fixed for entire loan tenure</summary>
    Fixed = 0,
    
    /// <summary>Interest rate changes based on market conditions</summary>
    Floating = 1,
    
    /// <summary>Hybrid - fixed for initial period, then floating</summary>
    HybridFixedFloating = 2
}
