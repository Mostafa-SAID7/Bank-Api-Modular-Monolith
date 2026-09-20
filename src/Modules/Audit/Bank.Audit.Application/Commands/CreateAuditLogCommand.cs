namespace Bank.Audit.Application.Commands;

using Bank.Audit.Domain.Enums;

public record CreateAuditLogCommand(
    string UserId,
    AuditEventType EventType,
    ResourceType ResourceType,
    string ResourceId,
    string Action,
    DateTime Timestamp,
    string IpAddress,
    string UserAgent,
    string? Details) : IRequest<Guid>;
