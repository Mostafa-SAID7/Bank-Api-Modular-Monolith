namespace Bank.Statements.Application.DTOs;

using Bank.Statements.Domain.Enums;

public sealed record StatementLineDto(
    Guid Id,
    DateTime TransactionDate,
    DateTime PostedDate,
    string Description,
    StatementLineType LineType,
    decimal Amount,
    decimal Balance,
    string? Reference,
    string? ChequeNumber);
