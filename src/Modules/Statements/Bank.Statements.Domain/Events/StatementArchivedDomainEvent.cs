namespace Bank.Statements.Domain.Events;

using Bank.BuildingBlocks.Domain.Abstractions;
using Bank.Statements.Domain.Enums;

public sealed record StatementArchivedDomainEvent(
    Guid StatementId,
    Guid CustomerId,
    Guid AccountId,
    ArchiveReason Reason) : DomainEvent(StatementId, "StatementArchived");

