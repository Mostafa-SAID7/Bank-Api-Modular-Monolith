namespace Bank.Cards.Domain.Entities;

/// <summary>
/// Represents a card transaction (purchase, ATM, etc.)
/// </summary>
public class CardTransaction
{
    public Guid Id { get; private set; }
    public Guid CardId { get; private set; }
    public TransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public decimal FeeAmount { get; private set; }
    public decimal NetAmount { get; private set; } // Amount + Fee
    public TransactionStatus Status { get; private set; }
    public string? DeclineReason { get; private set; }
    
    // Transaction details
    public string Merchant { get; private set; } = string.Empty;
    public string MerchantCategory { get; private set; } = string.Empty; // MCC code
    public bool IsInternational { get; private set; }
    public bool IsContactless { get; private set; }
    public string? AuthorizationCode { get; private set; }
    public string? ReferenceNumber { get; private set; } // Check number, reference code
    
    // Dates
    public DateTime TransactionDateUtc { get; private set; }
    public DateTime? PostingDateUtc { get; private set; } // When settled
    public DateTime CreatedAtUtc { get; private set; }
    
    // Fraud check
    public bool IsSuspicious { get; private set; }
    public string? SuspiciousReason { get; private set; }

    /// <summary>
    /// Create a new card transaction
    /// </summary>
    public static CardTransaction Create(
        Guid cardId,
        TransactionType type,
        decimal amount,
        string merchant,
        string merchantCategory,
        bool isInternational = false,
        bool isContactless = false,
        string? referenceNumber = null)
    {
        return new CardTransaction
        {
            Id = Guid.NewGuid(),
            CardId = cardId,
            Type = type,
            Amount = amount,
            FeeAmount = 0m,
            NetAmount = amount,
            Status = TransactionStatus.Pending,
            Merchant = merchant,
            MerchantCategory = merchantCategory,
            IsInternational = isInternational,
            IsContactless = isContactless,
            ReferenceNumber = referenceNumber,
            TransactionDateUtc = DateTime.UtcNow,
            CreatedAtUtc = DateTime.UtcNow,
            IsSuspicious = false,
        };
    }

    /// <summary>
    /// Approve the transaction
    /// </summary>
    public void Approve(string authorizationCode)
    {
        if (Status != TransactionStatus.Pending)
            throw new InvalidOperationException($"Cannot approve transaction in {Status} status");

        Status = TransactionStatus.Approved;
        AuthorizationCode = authorizationCode;
    }

    /// <summary>
    /// Decline the transaction
    /// </summary>
    public void Decline(string reason)
    {
        if (Status != TransactionStatus.Pending)
            throw new InvalidOperationException($"Cannot decline transaction in {Status} status");

        Status = TransactionStatus.Declined;
        DeclineReason = reason;
    }

    /// <summary>
    /// Reverse/refund the transaction
    /// </summary>
    public void Reverse(string reason = "")
    {
        if (Status != TransactionStatus.Approved)
            throw new InvalidOperationException($"Cannot reverse transaction in {Status} status");

        Status = TransactionStatus.Reversed;
        DeclineReason = reason;
        PostingDateUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Mark transaction as posted (settled)
    /// </summary>
    public void Post()
    {
        if (Status != TransactionStatus.Approved)
            throw new InvalidOperationException("Only approved transactions can be posted");

        PostingDateUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Apply a fee to the transaction
    /// </summary>
    public void ApplyFee(decimal feeAmount)
    {
        FeeAmount = feeAmount;
        NetAmount = Amount + FeeAmount;
    }

    /// <summary>
    /// Mark transaction as suspicious for review
    /// </summary>
    public void MarkSuspicious(string reason)
    {
        IsSuspicious = true;
        SuspiciousReason = reason;
        Status = TransactionStatus.Suspicious;
    }

    /// <summary>
    /// Clear suspicious flag
    /// </summary>
    public void ClearSuspicious()
    {
        IsSuspicious = false;
        SuspiciousReason = null;
    }
}
