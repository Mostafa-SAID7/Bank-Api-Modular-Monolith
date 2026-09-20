namespace Bank.Audit.Domain.Entities;

using Bank.Audit.Domain.Enums;
using Bank.Audit.Domain.Events;

public class AuditLog
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public AuditEventType EventType { get; private set; }
    public ResourceType ResourceType { get; private set; }
    public string ResourceId { get; private set; } = string.Empty;
    public string Action { get; private set; } = string.Empty;
    public DateTime Timestamp { get; private set; }
    public string IpAddress { get; private set; } = string.Empty;
    public string UserAgent { get; private set; } = string.Empty;
    public AuditStatus Status { get; private set; }
    public string? Details { get; private set; }

    private readonly List<object> _domainEvents = new();

    public IReadOnlyList<object> DomainEvents => _domainEvents.AsReadOnly();

    private AuditLog() { }

    public static AuditLog Create(
        string userId,
        AuditEventType eventType,
        ResourceType resourceType,
        string resourceId,
        string action,
        DateTime timestamp,
        string ipAddress,
        string userAgent,
        string? details = null)
    {
        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EventType = eventType,
            ResourceType = resourceType,
            ResourceId = resourceId,
            Action = action,
            Timestamp = timestamp,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Status = AuditStatus.Active,
            Details = details
        };

        auditLog.RaiseDomainEvent(new AuditLogCreatedDomainEvent(
            auditLog.Id,
            auditLog.UserId,
            auditLog.EventType,
            auditLog.ResourceType,
            auditLog.ResourceId,
            auditLog.Action,
            auditLog.Timestamp));

        return auditLog;
    }

    public void Archive()
    {
        Status = AuditStatus.Archived;
        RaiseDomainEvent(new AuditLogArchivedDomainEvent(Id, UserId, DateTime.UtcNow));
    }

    public void UpdateDetails(string? details)
    {
        Details = details;
        RaiseDomainEvent(new AuditLogUpdatedDomainEvent(Id, UserId, DateTime.UtcNow));
    }

    private void RaiseDomainEvent(object domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
