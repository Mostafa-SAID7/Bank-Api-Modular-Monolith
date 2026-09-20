namespace Bank.Cards.Domain.Enums;

/// <summary>
/// Type of card transaction
/// </summary>
public enum TransactionType
{
    /// <summary>Point-of-sale purchase</summary>
    Purchase = 0,
    
    /// <summary>ATM cash withdrawal</summary>
    AtmWithdrawal = 1,
    
    /// <summary>Transfer between accounts</summary>
    Transfer = 2,
    
    /// <summary>Refund or reversal</summary>
    Reversal = 3,
    
    /// <summary>Cash advance</summary>
    CashAdvance = 4,
    
    /// <summary>Balance inquiry (may have fee)</summary>
    BalanceInquiry = 5,
    
    /// <summary>Card fee or service charge</summary>
    Fee = 6,
    
    /// <summary>Interest charge (credit cards)</summary>
    Interest = 7
}
