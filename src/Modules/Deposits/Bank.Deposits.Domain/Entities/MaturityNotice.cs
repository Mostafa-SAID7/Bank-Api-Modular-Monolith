namespace Bank.Deposits.Domain.Entities;

/// <summary>
/// Represents a maturity notification for a fixed deposit
/// </summary>
public class MaturityNotice
{
    public Guid Id { get; private set; }
    public Guid FixedDepositId { get; private set; }
    public DateTime MaturityDateUtc { get; private set; }
    public DateTime NoticeDateUtc { get; private set; }
    public bool IsSent { get; private set; }
    public DateTime? SentAtUtc { get; private set; }
    public MaturityAction? CustomerChoice { get; private set; }
    public DateTime? ChoiceMadeDateUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    /// <summary>
    /// Create a new maturity notice
    /// </summary>
    public static MaturityNotice Create(
        Guid fixedDepositId,
        DateTime maturityDate)
    {
        if (maturityDate <= DateTime.UtcNow)
            throw new InvalidOperationException("Maturity date must be in the future");

        return new MaturityNotice
        {
            Id = Guid.NewGuid(),
            FixedDepositId = fixedDepositId,
            MaturityDateUtc = maturityDate,
            NoticeDateUtc = DateTime.UtcNow,
            IsSent = false,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        };
    }

    /// <summary>
    /// Mark notice as sent
    /// </summary>
    public void MarkAsSent()
    {
        if (IsSent)
            throw new InvalidOperationException("Notice has already been sent");
        
        IsSent = true;
        SentAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Record customer's maturity action choice
    /// </summary>
    public void RecordCustomerChoice(MaturityAction action)
    {
        CustomerChoice = action;
        ChoiceMadeDateUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
