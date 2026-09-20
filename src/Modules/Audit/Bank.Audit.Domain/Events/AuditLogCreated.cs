namespace Bank.Audit.Domain.Events;

using Bank.Audit.Domain.Enums;

public sealed record AuditLogCreatedDomainEvent(
    Guid AuditLogId,
    string UserId,
    AuditEventType AuditEventType,
    ResourceType ResourceType,
    string ResourceId,
    string Action,
    DateTime Timestamp) : DomainEvent(AuditLogId, "AuditLogCreated");
