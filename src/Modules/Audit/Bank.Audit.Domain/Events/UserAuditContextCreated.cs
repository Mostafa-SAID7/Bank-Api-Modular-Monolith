namespace Bank.Audit.Domain.Events;

public sealed record UserAuditContextCreatedDomainEvent(
    Guid ContextId,
    string UserId,
    DateTime CreatedAt) : DomainEvent(ContextId, "UserAuditContextCreated");
