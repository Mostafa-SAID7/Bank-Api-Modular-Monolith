namespace Bank.Statements.Presentation.Requests;

public sealed record DownloadStatementRequest(
    Guid StatementId,
    Guid CustomerId);
