namespace Bank.Cards.Domain.Entities;

/// <summary>
/// Card product definition (template for issued cards)
/// Examples: Premium Visa, Student Debit Card, etc.
/// </summary>
public class CardProduct
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public CardType CardType { get; private set; }
    public CardBrand Brand { get; private set; }
    
    // Fees
    public decimal AnnualFee { get; private set; }
    public decimal IssuanceFeePercentage { get; private set; }
    public decimal AtmFee { get; private set; }
    public decimal ForeignTransactionFeePercentage { get; private set; }
    
    // Limits
    public decimal DefaultDailyLimit { get; private set; }
    public decimal DefaultTransactionLimit { get; private set; }
    public int DailyTransactionCountLimit { get; private set; }
    
    // Features
    public bool RequiresPinForAtm { get; private set; }
    public bool AllowsContactless { get; private set; }
    public bool AllowsOnlineTransactions { get; private set; }
    public bool AllowsInternational { get; private set; }
    public bool RequiresPinForContactless { get; private set; }
    
    // Card validity
    public int CardValidityYears { get; private set; }
    public int ReplacementAllowancePerYear { get; private set; }
    
    // Status
    public bool IsActive { get; private set; }
    
    // Audit
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    /// <summary>
    /// Create a new card product
    /// </summary>
    public static CardProduct Create(
        string name,
        string description,
        CardType cardType,
        CardBrand brand,
        decimal annualFee,
        decimal issuanceFeePercentage,
        decimal defaultDailyLimit,
        decimal defaultTransactionLimit,
        int cardValidityYears,
        bool requiresPinForAtm = true,
        bool allowsContactless = true)
    {
        return new CardProduct
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            CardType = cardType,
            Brand = brand,
            AnnualFee = annualFee,
            IssuanceFeePercentage = issuanceFeePercentage,
            AtmFee = 0m,
            ForeignTransactionFeePercentage = 2.5m,
            
            DefaultDailyLimit = defaultDailyLimit,
            DefaultTransactionLimit = defaultTransactionLimit,
            DailyTransactionCountLimit = 50,
            
            RequiresPinForAtm = requiresPinForAtm,
            AllowsContactless = allowsContactless,
            AllowsOnlineTransactions = true,
            AllowsInternational = true,
            RequiresPinForContactless = false,
            
            CardValidityYears = cardValidityYears,
            ReplacementAllowancePerYear = 2,
            IsActive = true,
            
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        };
    }

    /// <summary>
    /// Deactivate the product (no new cards can be issued)
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Reactivate the product
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Update fees
    /// </summary>
    public void UpdateFees(
        decimal annualFee,
        decimal issuanceFeePercentage,
        decimal atmFee,
        decimal foreignFeePercentage)
    {
        AnnualFee = annualFee;
        IssuanceFeePercentage = issuanceFeePercentage;
        AtmFee = atmFee;
        ForeignTransactionFeePercentage = foreignFeePercentage;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Update default limits
    /// </summary>
    public void UpdateLimits(
        decimal dailyLimit,
        decimal transactionLimit,
        int transactionCountLimit)
    {
        DefaultDailyLimit = dailyLimit;
        DefaultTransactionLimit = transactionLimit;
        DailyTransactionCountLimit = transactionCountLimit;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
