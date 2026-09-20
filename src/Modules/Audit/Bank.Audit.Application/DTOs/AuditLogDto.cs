namespace Bank.Audit.Application.DTOs;

using Bank.Audit.Domain.Enums;

public record AuditLogDto(
    Guid Id,
    string UserId,
    AuditEventType EventType,
    ResourceType ResourceType,
    string ResourceId,
    string Action,
    DateTime Timestamp,
    string IpAddress,
    string UserAgent,
    AuditStatus Status,
    string? Details);
