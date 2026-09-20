namespace Bank.Audit.Presentation.Requests;

using Bank.Audit.Domain.Enums;

public record CreateAuditLogRequest(
    string UserId,
    string EventType,
    string ResourceType,
    string ResourceId,
    string Action,
    DateTime Timestamp,
    string IpAddress,
    string UserAgent,
    string? Details);
