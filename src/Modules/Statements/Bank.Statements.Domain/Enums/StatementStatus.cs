namespace Bank.Statements.Domain.Enums;

public enum StatementStatus
{
    Pending = 0,
    Generated = 1,
    Sent = 2,
    Downloaded = 3,
    Archived = 4,
    Cancelled = 5
}
