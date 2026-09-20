namespace Bank.Cards.Domain.Enums;

/// <summary>
/// Status of a card transaction
/// </summary>
public enum TransactionStatus
{
    /// <summary>Transaction is pending authorization</summary>
    Pending = 0,
    
    /// <summary>Transaction has been approved</summary>
    Approved = 1,
    
    /// <summary>Transaction has been declined</summary>
    Declined = 2,
    
    /// <summary>Transaction has been reversed/refunded</summary>
    Reversed = 3,
    
    /// <summary>Transaction failed to process</summary>
    Failed = 4,
    
    /// <summary>Transaction is being investigated</summary>
    Suspicious = 5
}
