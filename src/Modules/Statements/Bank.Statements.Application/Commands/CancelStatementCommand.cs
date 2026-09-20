namespace Bank.Statements.Application.Commands;

public sealed record CancelStatementCommand(
    Guid StatementId,
    string Reason);
