using Bank.CoreBanking.Domain.Enums;

namespace Bank.CoreBanking.Domain.Entities;

/// <summary>
/// Account aggregate root - owns balance, holds, and restrictions
/// Migrated from Bank.Domain - responsible for posting operations
/// </summary>
public class Account
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountHolderName { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; } = "USD"; // ISO 4217 currency code
    
    // Account lifecycle
    public AccountStatus Status { get; set; } = AccountStatus.Active;
    public AccountType Type { get; set; } = AccountType.Checking;
    public DateTime OpenedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ClosedDate { get; set; }
    public string? ClosureReason { get; set; }
    
    // Dormancy
    public DateTime LastActivityDate { get; set; } = DateTime.UtcNow;
    public DateTime? DormancyDate { get; set; }
    public int DormancyPeriodDays { get; set; } = 365;
    
    // Fee management
    public decimal MinimumBalance { get; set; }
    public decimal MonthlyMaintenanceFee { get; set; }
    public bool FeeWaiverEligible { get; set; }
    public DateTime? LastFeeCalculationDate { get; set; }
    
    // Interest
    public decimal InterestRate { get; set; }
    public DateTime? LastInterestCalculationDate { get; set; }
    
    // Joint account
    public bool IsJointAccount { get; set; }
    public bool RequiresMultipleSignatures { get; set; }
    
    // Holds & Restrictions
    public bool HasHolds { get; set; }
    public bool HasRestrictions { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    private Account() { }

    public static Account Create(
        string accountNumber,
        string accountHolderName,
        Guid customerId,
        string currency = "USD",
        AccountType type = AccountType.Checking,
        decimal initialBalance = 0)
    {
        return new Account
        {
            Id = Guid.NewGuid(),
            AccountNumber = accountNumber,
            AccountHolderName = accountHolderName,
            CustomerId = customerId,
            Currency = currency,
            Type = type,
            Balance = initialBalance,
            Status = AccountStatus.Active,
            OpenedAtUtc = DateTime.UtcNow,
            LastActivityDate = DateTime.UtcNow,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Post an amount to this account (credit or debit)
    /// Returns the new balance after posting
    /// </summary>
    public decimal PostTransaction(decimal amount)
    {
        if (Status != AccountStatus.Active)
            throw new InvalidOperationException($"Cannot post to account with status {Status}");

        Balance += amount;
        LastActivityDate = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;

        return Balance;
    }

    public void Close(string reason)
    {
        Status = AccountStatus.Closed;
        ClosedDate = DateTime.UtcNow;
        ClosureReason = reason;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Suspend()
    {
        Status = AccountStatus.Suspended;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Reactivate()
    {
        if (Status == AccountStatus.Closed)
            throw new InvalidOperationException("Cannot reactivate a closed account");

        Status = AccountStatus.Active;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
