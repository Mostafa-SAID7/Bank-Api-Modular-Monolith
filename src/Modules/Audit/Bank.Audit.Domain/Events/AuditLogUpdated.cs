namespace Bank.Audit.Domain.Events;

public sealed record AuditLogUpdatedDomainEvent(
    Guid AuditLogId,
    string UserId,
    DateTime UpdatedAt) : DomainEvent(AuditLogId, "AuditLogUpdated");
