namespace Bank.Deposits.Domain.Enums;

/// <summary>
/// Represents the action to be taken when a fixed deposit reaches maturity
/// </summary>
public enum MaturityAction
{
    /// <summary>Deposit is paid out to the linked account</summary>
    Payout = 1,

    /// <summary>Deposit is automatically renewed for another term</summary>
    Renewal = 2,

    /// <summary>Principal is paid out; interest is reinvested in new deposit</summary>
    Reinvestment = 3,
}
