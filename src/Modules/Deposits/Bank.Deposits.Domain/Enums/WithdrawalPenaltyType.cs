namespace Bank.Deposits.Domain.Enums;

/// <summary>
/// Represents the type of penalty applied for early withdrawal from a fixed deposit
/// </summary>
public enum WithdrawalPenaltyType
{
    /// <summary>No penalty for early withdrawal</summary>
    None = 0,

    /// <summary>Fixed amount penalty regardless of withdrawal amount</summary>
    FixedAmount = 1,

    /// <summary>Percentage-based penalty on the withdrawal amount</summary>
    Percentage = 2,

    /// <summary>Forfeiture of accrued interest</summary>
    InterestForfeiture = 3,

    /// <summary>Combined fixed amount and percentage penalty</summary>
    Combined = 4,
}
