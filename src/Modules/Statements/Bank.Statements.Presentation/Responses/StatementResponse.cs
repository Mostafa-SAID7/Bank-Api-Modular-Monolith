namespace Bank.Statements.Presentation.Responses;

public sealed record StatementResponse(
    Guid Id,
    Guid CustomerId,
    Guid AccountId,
    DateTime StatementDate,
    DateTime PeriodStartDate,
    DateTime PeriodEndDate,
    decimal OpeningBalance,
    decimal ClosingBalance,
    decimal TotalDebits,
    decimal TotalCredits,
    string Status,
    string Period,
    string Format,
    DateTime CreatedAt,
    DateTime? GeneratedAt,
    DateTime? SentAt,
    DateTime? DownloadedAt);
