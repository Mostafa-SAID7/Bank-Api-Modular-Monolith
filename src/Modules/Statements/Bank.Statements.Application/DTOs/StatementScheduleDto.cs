namespace Bank.Statements.Application.DTOs;

using Bank.Statements.Domain.Enums;

public sealed record StatementScheduleDto(
    Guid Id,
    Guid CustomerId,
    Guid AccountId,
    StatementPeriod Period,
    StatementFormat Format,
    DeliveryMethod DeliveryMethod,
    bool IsActive,
    DateTime? NextGenerationDate,
    DateTime? LastGeneratedDate,
    DateTime CreatedAt);
