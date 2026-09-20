namespace Bank.Audit.Domain.Events;

using Bank.Audit.Domain.Enums;

public sealed record AuditEntryRecordedDomainEvent(
    Guid AuditEntryId,
    Guid AuditLogId,
    string EntityName,
    ActionType ActionType,
    DateTime RecordedAt) : DomainEvent(AuditEntryId, "AuditEntryRecorded");
