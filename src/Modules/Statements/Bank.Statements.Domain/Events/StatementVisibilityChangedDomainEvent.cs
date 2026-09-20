namespace Bank.Statements.Domain.Events;

using Bank.BuildingBlocks.Domain.Abstractions;
using Bank.Statements.Domain.Enums;

public sealed record StatementVisibilityChangedDomainEvent(
    Guid StatementId,
    Guid CustomerId,
    Guid AccountId,
    StatementVisibility Visibility) : DomainEvent(StatementId, "StatementVisibilityChanged");

