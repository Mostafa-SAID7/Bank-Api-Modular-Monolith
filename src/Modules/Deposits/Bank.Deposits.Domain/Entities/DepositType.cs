namespace Bank.Deposits.Domain.Entities;

/// <summary>
/// DepositType aggregate - Represents types of deposits available (Savings, Fixed, Recurring, etc.)
/// </summary>
public class DepositType
{
    /// <summary>Unique identifier for deposit type</summary>
    public Guid Id { get; set; }

    /// <summary>Name of the deposit type (e.g., "Savings Account", "Fixed Deposit 1-Year")</summary>
    public string Name { get; set; } = null!;

    /// <summary>Description of deposit type features and terms</summary>
    public string Description { get; set; } = null!;

    /// <summary>Minimum opening balance required</summary>
    public decimal MinimumBalance { get; set; }

    /// <summary>Maximum balance allowed (0 if no limit)</summary>
    public decimal MaximumBalance { get; set; }

    /// <summary>Is this a fixed deposit type (has maturity term)</summary>
    public bool IsFixedDeposit { get; set; }

    /// <summary>Default term in months for fixed deposits (0 if not applicable)</summary>
    public int DefaultTermMonths { get; set; }

    /// <summary>Can early withdrawal be done (for fixed deposits)</summary>
    public bool AllowsEarlyWithdrawal { get; set; }

    /// <summary>Penalty percentage for early withdrawal</summary>
    public decimal EarlyWithdrawalPenaltyPercent { get; set; }

    /// <summary>Is premature closure allowed</summary>
    public bool AllowsPrematureClosure { get; set; }

    /// <summary>Penalty percentage for premature closure</summary>
    public decimal PrematureClosurePenaltyPercent { get; set; }

    /// <summary>Whether deposits are active and accepting new accounts</summary>
    public bool IsActive { get; set; }

    /// <summary>Creation timestamp</summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>Last update timestamp</summary>
    public DateTime UpdatedAtUtc { get; set; }
}
