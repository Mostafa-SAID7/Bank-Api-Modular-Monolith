namespace Bank.Statements.Application.Commands;

public sealed record DownloadStatementCommand(
    Guid StatementId,
    Guid CustomerId);
