namespace Bank.Statements.Application.Commands;

using Bank.Statements.Domain.Enums;

public sealed record CreateScheduleCommand(
    Guid CustomerId,
    Guid AccountId,
    StatementPeriod Period,
    StatementFormat Format,
    DeliveryMethod DeliveryMethod);
