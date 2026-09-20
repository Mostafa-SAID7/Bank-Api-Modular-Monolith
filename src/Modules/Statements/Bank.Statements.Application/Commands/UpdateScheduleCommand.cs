namespace Bank.Statements.Application.Commands;

using Bank.Statements.Domain.Enums;

public sealed record UpdateScheduleCommand(
    Guid ScheduleId,
    StatementPeriod Period,
    StatementFormat Format,
    DeliveryMethod DeliveryMethod);
