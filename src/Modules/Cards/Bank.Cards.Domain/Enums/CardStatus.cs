namespace Bank.Cards.Domain.Enums;

/// <summary>
/// Status of a card throughout its lifecycle
/// </summary>
public enum CardStatus
{
    /// <summary>Card created but not activated yet</summary>
    Inactive = 0,
    
    /// <summary>Card is active and usable</summary>
    Active = 1,
    
    /// <summary>Card is blocked (lost, stolen, fraud, etc.)</summary>
    Blocked = 2,
    
    /// <summary>Card has expired</summary>
    Expired = 3,
    
    /// <summary>Card has been closed</summary>
    Closed = 4,
    
    /// <summary>Card is suspended pending review</summary>
    Suspended = 5
}
