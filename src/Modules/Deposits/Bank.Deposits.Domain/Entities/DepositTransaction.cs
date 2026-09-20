namespace Bank.Deposits.Domain.Entities;

/// <summary>
/// Represents a transaction on a fixed deposit account
/// </summary>
public class DepositTransaction
{
    public Guid Id { get; private set; }
    public Guid FixedDepositId { get; private set; }
    public decimal Amount { get; private set; }
    public string TransactionType { get; private set; } = string.Empty;
    public DateTime TransactionDateUtc { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string? ReferenceNumber { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    /// <summary>
    /// Create a new deposit transaction
    /// </summary>
    public static DepositTransaction Create(
        Guid fixedDepositId,
        decimal amount,
        string transactionType,
        string description,
        string? referenceNumber = null)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Transaction amount must be greater than zero");

        if (string.IsNullOrWhiteSpace(transactionType))
            throw new InvalidOperationException("Transaction type is required");

        return new DepositTransaction
        {
            Id = Guid.NewGuid(),
            FixedDepositId = fixedDepositId,
            Amount = amount,
            TransactionType = transactionType,
            TransactionDateUtc = DateTime.UtcNow,
            Description = description,
            ReferenceNumber = referenceNumber,
            CreatedAtUtc = DateTime.UtcNow,
        };
    }
}
