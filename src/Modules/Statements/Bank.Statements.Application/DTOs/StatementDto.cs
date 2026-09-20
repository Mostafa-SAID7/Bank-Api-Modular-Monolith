namespace Bank.Statements.Application.DTOs;

using Bank.Statements.Domain.Enums;

public sealed record StatementDto(
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
    StatementStatus Status,
    StatementPeriod Period,
    StatementFormat Format,
    DateTime CreatedAt,
    DateTime? GeneratedAt,
    DateTime? SentAt,
    DateTime? DownloadedAt);
