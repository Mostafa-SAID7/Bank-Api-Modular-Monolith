namespace Bank.Audit.Domain.Events;

public sealed record UserAuditContextDeactivatedDomainEvent(
    Guid ContextId,
    string UserId,
    DateTime DeactivatedAt) : DomainEvent(ContextId, "UserAuditContextDeactivated");
