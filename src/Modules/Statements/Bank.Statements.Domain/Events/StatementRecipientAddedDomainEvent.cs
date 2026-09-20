namespace Bank.Statements.Domain.Events;

using Bank.BuildingBlocks.Domain.Abstractions;

public sealed record StatementRecipientAddedDomainEvent(
    Guid StatementId,
    Guid CustomerId,
    string RecipientEmail) : DomainEvent(StatementId, "StatementRecipientAdded");

