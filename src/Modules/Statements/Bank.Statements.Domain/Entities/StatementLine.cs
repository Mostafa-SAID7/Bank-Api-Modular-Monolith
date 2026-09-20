namespace Bank.Statements.Domain.Entities;

using Bank.Statements.Domain.Enums;

public class StatementLine
{
    public Guid Id { get; private set; }
    public Guid StatementId { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public DateTime PostedDate { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public StatementLineType LineType { get; private set; }
    public decimal Amount { get; private set; }
    public decimal Balance { get; private set; }
    public string? Reference { get; private set; }
    public string? ChequeNumber { get; private set; }
    public bool IsPosted { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private StatementLine() { }

    public static StatementLine Create(
        Guid statementId,
        DateTime transactionDate,
        DateTime postedDate,
        string description,
        StatementLineType lineType,
        decimal amount,
        decimal balance,
        string? reference = null,
        string? chequeNumber = null)
    {
        return new StatementLine
        {
            Id = Guid.NewGuid(),
            StatementId = statementId,
            TransactionDate = transactionDate,
            PostedDate = postedDate,
            Description = description,
            LineType = lineType,
            Amount = amount,
            Balance = balance,
            Reference = reference,
            ChequeNumber = chequeNumber,
            IsPosted = true,
            CreatedAt = DateTime.UtcNow
        };
    }
}
