namespace Bank.Statements.Domain.Events;

using Bank.BuildingBlocks.Domain.Abstractions;

public sealed record StatementGeneratedDomainEvent(
    Guid StatementId,
    Guid CustomerId,
    Guid AccountId,
    decimal ClosingBalance) : DomainEvent(StatementId, "StatementGenerated");

