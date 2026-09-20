namespace Bank.Statements.Domain.Events;

using Bank.BuildingBlocks.Domain.Abstractions;

public sealed record ScheduleDeactivatedDomainEvent(
    Guid ScheduleId,
    Guid CustomerId,
    Guid AccountId) : DomainEvent(ScheduleId, "ScheduleDeactivated");

