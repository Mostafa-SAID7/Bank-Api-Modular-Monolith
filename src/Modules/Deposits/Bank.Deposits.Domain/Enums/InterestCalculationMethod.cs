namespace Bank.Deposits.Domain.Enums;

/// <summary>
/// Represents the method used to calculate interest on a fixed deposit
/// </summary>
public enum InterestCalculationMethod
{
    /// <summary>Simple interest: Principal × Rate × Time</summary>
    Simple = 1,

    /// <summary>Compound interest calculated daily</summary>
    CompoundDaily = 2,

    /// <summary>Compound interest calculated monthly</summary>
    CompoundMonthly = 3,
}
