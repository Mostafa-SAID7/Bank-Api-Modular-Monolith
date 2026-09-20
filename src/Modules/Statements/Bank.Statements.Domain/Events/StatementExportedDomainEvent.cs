namespace Bank.Statements.Domain.Events;

using Bank.BuildingBlocks.Domain.Abstractions;

public sealed record StatementExportedDomainEvent(
    Guid StatementId,
    Guid CustomerId,
    Guid AccountId,
    string ExportReference) : DomainEvent(StatementId, "StatementExported");

