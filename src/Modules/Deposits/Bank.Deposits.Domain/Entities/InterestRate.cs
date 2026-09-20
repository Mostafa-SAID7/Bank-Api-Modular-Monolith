using Bank.Deposits.Domain.Enums;

namespace Bank.Deposits.Domain.Entities;

/// <summary>
/// InterestRate entity - Represents interest rate configuration for a deposit type
/// </summary>
public class InterestRate
{
    /// <summary>Unique identifier</summary>
    public Guid Id { get; set; }

    /// <summary>Reference to deposit type</summary>
    public Guid DepositTypeId { get; set; }

    /// <summary>Navigation property to deposit type</summary>
    public DepositType DepositType { get; set; } = null!;

    /// <summary>Annual interest rate as percentage (e.g., 5.5 for 5.5%)</summary>
    public decimal AnnualRate { get; set; }

    /// <summary>Effective date when this rate becomes applicable</summary>
    public DateTime EffectiveFromUtc { get; set; }

    /// <summary>Date when this rate is no longer applicable (null if current)</summary>
    public DateTime? EffectiveToUtc { get; set; }

    /// <summary>Minimum balance required to earn this rate</summary>
    public decimal MinimumBalanceForRate { get; set; }

    /// <summary>Maximum balance up to which rate applies</summary>
    public decimal MaximumBalanceForRate { get; set; }

    /// <summary>Frequency of interest payment/compounding</summary>
    public InterestFrequency Frequency { get; set; }

    /// <summary>Method for calculating interest (simple or compound)</summary>
    public InterestCalculationMethod CalculationMethod { get; set; }

    /// <summary>Whether this rate is currently active</summary>
    public bool IsActive { get; set; }

    /// <summary>Creation timestamp</summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>Last update timestamp</summary>
    public DateTime UpdatedAtUtc { get; set; }
}
