namespace Bank.Loans.Domain.Enums;

/// <summary>
/// EMI (Equated Monthly Installment) payment frequency enumeration
/// </summary>
public enum EMIFrequency
{
    /// <summary>Payment due monthly</summary>
    Monthly = 0,
    
    /// <summary>Payment due quarterly</summary>
    Quarterly = 1,
    
    /// <summary>Payment due semi-annually</summary>
    SemiAnnually = 2,
    
    /// <summary>Payment due annually</summary>
    Annually = 3,
    
    /// <summary>Payment due bi-weekly</summary>
    BiWeekly = 4
}
