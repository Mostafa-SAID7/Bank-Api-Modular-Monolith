namespace Bank.Deposits.Domain.Enums;

/// <summary>
/// Method for calculating deposit interest
/// </summary>
public enum InterestCalculationMethod
{
    /// <summary>Simple interest calculation (principal * rate * time)</summary>
    Simple = 0,

    /// <summary>Compound interest calculation</summary>
    Compound = 1
}
