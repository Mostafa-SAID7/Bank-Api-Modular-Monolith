namespace Bank.Statements.Application.Commands;

public sealed record SendStatementCommand(
    Guid StatementId);
