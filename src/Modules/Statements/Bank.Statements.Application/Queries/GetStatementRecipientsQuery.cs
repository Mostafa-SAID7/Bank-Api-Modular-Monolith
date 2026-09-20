namespace Bank.Statements.Application.Queries;

public sealed record GetStatementRecipientsQuery(
    Guid StatementId);
