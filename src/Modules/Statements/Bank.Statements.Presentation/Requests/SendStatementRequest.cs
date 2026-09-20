namespace Bank.Statements.Presentation.Requests;

public sealed record SendStatementRequest(
    Guid StatementId);
