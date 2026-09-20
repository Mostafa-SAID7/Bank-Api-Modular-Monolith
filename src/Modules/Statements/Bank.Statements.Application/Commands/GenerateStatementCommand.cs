namespace Bank.Statements.Application.Commands;

public sealed record GenerateStatementCommand(
    Guid CustomerId,
    Guid AccountId,
    DateTime PeriodStartDate,
    DateTime PeriodEndDate,
    decimal OpeningBalance);
