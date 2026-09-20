namespace Bank.Statements.Domain.Events;

using Bank.BuildingBlocks.Domain.Abstractions;

public sealed record StatementCancelledDomainEvent(
    Guid StatementId,
    Guid CustomerId,
    Guid AccountId,
    string Reason) : DomainEvent(StatementId, "StatementCancelled");

