namespace Bank.Cards.Domain.Enums;

/// <summary>
/// Reason for blocking a card
/// </summary>
public enum BlockReason
{
    /// <summary>Card reported lost</summary>
    Lost = 0,
    
    /// <summary>Card reported stolen</summary>
    Stolen = 1,
    
    /// <summary>Fraudulent activity detected</summary>
    Fraud = 2,
    
    /// <summary>Customer requested block</summary>
    CustomerRequest = 3,
    
    /// <summary>Card expired</summary>
    Expired = 4,
    
    /// <summary>Suspicious activity detected by system</summary>
    SuspiciousActivity = 5,
    
    /// <summary>Administrative action</summary>
    Administrative = 6,
    
    /// <summary>Account closure</summary>
    AccountClosure = 7
}
