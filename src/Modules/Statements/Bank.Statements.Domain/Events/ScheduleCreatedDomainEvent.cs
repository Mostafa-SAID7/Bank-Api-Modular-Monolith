namespace Bank.Statements.Domain.Events;

using Bank.BuildingBlocks.Domain.Abstractions;

public sealed record ScheduleCreatedDomainEvent(
    Guid ScheduleId,
    Guid CustomerId,
    Guid AccountId) : DomainEvent(ScheduleId, "ScheduleCreated");

