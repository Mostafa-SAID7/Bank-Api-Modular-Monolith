namespace Bank.Deposits.Domain.Enums;

/// <summary>
/// Interest payment/compounding frequency for deposits
/// </summary>
public enum InterestFrequency
{
    /// <summary>Interest calculated and paid daily</summary>
    Daily = 0,

    /// <summary>Interest calculated and paid weekly</summary>
    Weekly = 1,

    /// <summary>Interest calculated and paid monthly</summary>
    Monthly = 2,

    /// <summary>Interest calculated and paid quarterly</summary>
    Quarterly = 3,

    /// <summary>Interest calculated and paid semi-annually</summary>
    SemiAnnually = 4,

    /// <summary>Interest calculated and paid annually</summary>
    Annually = 5,

    /// <summary>Interest compounded and paid at maturity</summary>
    AtMaturity = 6
}
