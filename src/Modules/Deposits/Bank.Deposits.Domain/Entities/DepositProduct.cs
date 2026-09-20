namespace Bank.Deposits.Domain.Entities;

/// <summary>
/// Represents a fixed deposit product offering with configuration and tiering
/// </summary>
public class DepositProduct
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal MinimumAmount { get; private set; }
    public decimal MaximumAmount { get; private set; }
    public int MinimumTermDays { get; private set; }
    public int MaximumTermDays { get; private set; }
    public decimal BaseInterestRate { get; private set; }
    public InterestCalculationMethod InterestCalculationMethod { get; private set; }
    public InterestFrequency CompoundingFrequency { get; private set; }
    public bool AutoRenewalEnabled { get; private set; }
    public int AutoRenewalNoticeDays { get; private set; } = 30;
    public WithdrawalPenaltyType DefaultPenaltyType { get; private set; }
    public decimal? DefaultPenaltyAmount { get; private set; }
    public decimal? DefaultPenaltyPercentage { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    /// <summary>
    /// Create a new deposit product
    /// </summary>
    public static DepositProduct Create(
        string name,
        string description,
        decimal minimumAmount,
        decimal maximumAmount,
        int minimumTermDays,
        int maximumTermDays,
        decimal baseInterestRate,
        InterestCalculationMethod calculationMethod,
        InterestFrequency compoundingFrequency,
        WithdrawalPenaltyType defaultPenaltyType,
        decimal? defaultPenaltyAmount = null,
        decimal? defaultPenaltyPercentage = null)
    {
        if (minimumAmount <= 0)
            throw new InvalidOperationException("Minimum amount must be greater than zero");
        
        if (maximumAmount < minimumAmount)
            throw new InvalidOperationException("Maximum amount must be greater than or equal to minimum amount");
        
        if (minimumTermDays <= 0)
            throw new InvalidOperationException("Minimum term must be greater than zero");
        
        if (maximumTermDays < minimumTermDays)
            throw new InvalidOperationException("Maximum term must be greater than or equal to minimum term");
        
        if (baseInterestRate < 0)
            throw new InvalidOperationException("Interest rate cannot be negative");

        return new DepositProduct
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            MinimumAmount = minimumAmount,
            MaximumAmount = maximumAmount,
            MinimumTermDays = minimumTermDays,
            MaximumTermDays = maximumTermDays,
            BaseInterestRate = baseInterestRate,
            InterestCalculationMethod = calculationMethod,
            CompoundingFrequency = compoundingFrequency,
            AutoRenewalEnabled = true,
            DefaultPenaltyType = defaultPenaltyType,
            DefaultPenaltyAmount = defaultPenaltyAmount,
            DefaultPenaltyPercentage = defaultPenaltyPercentage,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        };
    }

    /// <summary>
    /// Validate if amount is within product limits
    /// </summary>
    public bool IsAmountValid(decimal amount)
    {
        return amount >= MinimumAmount && amount <= MaximumAmount;
    }

    /// <summary>
    /// Validate if term is within product limits
    /// </summary>
    public bool IsTermValid(int termDays)
    {
        return termDays >= MinimumTermDays && termDays <= MaximumTermDays;
    }

    /// <summary>
    /// Deactivate the product
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Activate the product
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
