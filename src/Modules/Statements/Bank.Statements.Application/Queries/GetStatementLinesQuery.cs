namespace Bank.Statements.Application.Queries;

public sealed record GetStatementLinesQuery(
    Guid StatementId,
    int Page = 1,
    int PageSize = 50);
