namespace Bank.Statements.Domain.Events;

using Bank.BuildingBlocks.Domain.Abstractions;

public sealed record StatementDownloadedDomainEvent(
    Guid StatementId,
    Guid CustomerId,
    Guid AccountId) : DomainEvent(StatementId, "StatementDownloaded");

