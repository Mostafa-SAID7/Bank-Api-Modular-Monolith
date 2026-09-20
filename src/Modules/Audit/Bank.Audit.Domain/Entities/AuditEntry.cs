namespace Bank.Audit.Domain.Entities;

using Bank.Audit.Domain.Enums;
using Bank.Audit.Domain.Events;

public class AuditEntry
{
    public Guid Id { get; private set; }
    public Guid AuditLogId { get; private set; }
    public string EntityName { get; private set; } = string.Empty;
    public ActionType ActionType { get; private set; }
    public string OldValues { get; private set; } = string.Empty;
    public string NewValues { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public EntityChangeType ChangeType { get; private set; }

    private readonly List<object> _domainEvents = new();

    public IReadOnlyList<object> DomainEvents => _domainEvents.AsReadOnly();

    private AuditEntry() { }

    public static AuditEntry Create(
        Guid auditLogId,
        string entityName,
        ActionType actionType,
        string oldValues,
        string newValues,
        EntityChangeType changeType)
    {
        var entry = new AuditEntry
        {
            Id = Guid.NewGuid(),
            AuditLogId = auditLogId,
            EntityName = entityName,
            ActionType = actionType,
            OldValues = oldValues,
            NewValues = newValues,
            CreatedAt = DateTime.UtcNow,
            ChangeType = changeType
        };

        entry.RaiseDomainEvent(new AuditEntryRecordedDomainEvent(
            entry.Id,
            entry.AuditLogId,
            entry.EntityName,
            entry.ActionType,
            entry.CreatedAt));

        return entry;
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
