namespace Bank.Statements.Application.Queries;

public sealed record GetCustomerStatementsQuery(
    Guid CustomerId,
    int Page = 1,
    int PageSize = 20);
