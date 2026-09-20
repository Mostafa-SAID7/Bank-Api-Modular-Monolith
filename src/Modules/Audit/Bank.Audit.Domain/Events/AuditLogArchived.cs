namespace Bank.Audit.Domain.Events;

public sealed record AuditLogArchivedDomainEvent(
    Guid AuditLogId,
    string UserId,
    DateTime ArchivedAt) : DomainEvent(AuditLogId, "AuditLogArchived");
