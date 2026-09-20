namespace Bank.Audit.Presentation.Responses;

using Bank.Audit.Domain.Enums;

public record GetAuditLogResponse(
    Guid Id,
    string UserId,
    string EventType,
    string ResourceType,
    string ResourceId,
    string Action,
    DateTime Timestamp,
    string IpAddress,
    string UserAgent,
    string Status,
    string? Details);
