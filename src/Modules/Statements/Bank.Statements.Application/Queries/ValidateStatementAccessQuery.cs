namespace Bank.Statements.Application.Queries;

public sealed record ValidateStatementAccessQuery(
    Guid StatementId,
    Guid CustomerId);
