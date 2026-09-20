namespace Bank.Audit.Application.DTOs;

using Bank.Audit.Domain.Enums;

public record AuditEntryDto(
    Guid Id,
    Guid AuditLogId,
    string EntityName,
    ActionType ActionType,
    string OldValues,
    string NewValues,
    DateTime CreatedAt,
    EntityChangeType ChangeType);
