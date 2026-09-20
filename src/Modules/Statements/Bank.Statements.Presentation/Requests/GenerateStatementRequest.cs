namespace Bank.Statements.Presentation.Requests;

public sealed record GenerateStatementRequest(
    Guid CustomerId,
    Guid AccountId,
    DateTime PeriodStartDate,
    DateTime PeriodEndDate,
    decimal OpeningBalance);
