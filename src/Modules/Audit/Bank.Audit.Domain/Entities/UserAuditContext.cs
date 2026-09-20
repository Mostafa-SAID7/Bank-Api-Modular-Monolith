namespace Bank.Audit.Domain.Entities;

using Bank.Audit.Domain.Events;

public class UserAuditContext
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public int AuditLogCount { get; private set; }
    public DateTime LastActivityAt { get; private set; }
    public string LastActivityDescription { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    private readonly List<object> _domainEvents = new();

    public IReadOnlyList<object> DomainEvents => _domainEvents.AsReadOnly();

    private UserAuditContext() { }

    public static UserAuditContext Create(
        string userId,
        string lastActivityDescription)
    {
        var context = new UserAuditContext
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AuditLogCount = 0,
            LastActivityAt = DateTime.UtcNow,
            LastActivityDescription = lastActivityDescription,
            IsActive = true
        };

        context.RaiseDomainEvent(new UserAuditContextCreatedDomainEvent(
            context.Id,
            context.UserId,
            context.LastActivityAt));

        return context;
    }

    public void IncrementAuditLogCount()
    {
        AuditLogCount++;
        LastActivityAt = DateTime.UtcNow;
    }

    public void UpdateLastActivity(string description)
    {
        LastActivityDescription = description;
        LastActivityAt = DateTime.UtcNow;
        RaiseDomainEvent(new UserActivityRecordedDomainEvent(Id, UserId, LastActivityAt));
    }

    public void Deactivate()
    {
        IsActive = false;
        RaiseDomainEvent(new UserAuditContextDeactivatedDomainEvent(Id, UserId, DateTime.UtcNow));
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
