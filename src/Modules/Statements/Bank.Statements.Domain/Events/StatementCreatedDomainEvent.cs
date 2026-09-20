namespace Bank.Statements.Domain.Events;

using Bank.BuildingBlocks.Domain.Abstractions;

public sealed record StatementCreatedDomainEvent(
    Guid StatementId,
    Guid CustomerId,
    Guid AccountId) : DomainEvent(StatementId, "StatementCreated");

