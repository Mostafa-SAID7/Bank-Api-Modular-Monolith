namespace Bank.Cards.Domain.Enums;

/// <summary>
/// Type of card issued
/// </summary>
public enum CardType
{
    /// <summary>Debit card - draws from account balance</summary>
    Debit = 0,
    
    /// <summary>Credit card - revolving credit line</summary>
    Credit = 1,
    
    /// <summary>Prepaid card - balance-based</summary>
    Prepaid = 2
}
