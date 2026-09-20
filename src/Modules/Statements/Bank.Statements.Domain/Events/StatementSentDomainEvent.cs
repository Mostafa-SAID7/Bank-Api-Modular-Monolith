namespace Bank.Statements.Domain.Events;

using Bank.BuildingBlocks.Domain.Abstractions;

public sealed record StatementSentDomainEvent(
    Guid StatementId,
    Guid CustomerId,
    Guid AccountId) : DomainEvent(StatementId, "StatementSent");

