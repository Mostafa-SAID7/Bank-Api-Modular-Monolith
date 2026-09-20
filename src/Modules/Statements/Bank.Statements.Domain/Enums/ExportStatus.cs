namespace Bank.Statements.Domain.Enums;

public enum ExportStatus
{
    NotExported = 0,
    Exporting = 1,
    Exported = 2,
    ExportFailed = 3
}
