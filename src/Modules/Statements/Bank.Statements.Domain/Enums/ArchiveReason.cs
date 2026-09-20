namespace Bank.Statements.Domain.Enums;

public enum ArchiveReason
{
    UserInitiated = 0,
    RetentionPolicy = 1,
    AccountClosure = 2,
    Compliance = 3
}
