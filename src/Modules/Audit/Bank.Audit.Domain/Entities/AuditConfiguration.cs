namespace Bank.Audit.Domain.Entities;

using Bank.Audit.Domain.Enums;
using Bank.Audit.Domain.Events;

public class AuditConfiguration
{
    public Guid Id { get; private set; }
    public AuditLevel AuditLevel { get; private set; }
    public int RetentionDays { get; private set; }
    public bool EnableEmailNotifications { get; private set; }
    public string NotificationEmail { get; private set; } = string.Empty;
    public DateTime LastModified { get; private set; }
    public string ModifiedBy { get; private set; } = string.Empty;

    private readonly List<object> _domainEvents = new();

    public IReadOnlyList<object> DomainEvents => _domainEvents.AsReadOnly();

    private AuditConfiguration() { }

    public static AuditConfiguration Create(
        AuditLevel auditLevel,
        int retentionDays,
        bool enableEmailNotifications,
        string notificationEmail,
        string modifiedBy)
    {
        var config = new AuditConfiguration
        {
            Id = Guid.NewGuid(),
            AuditLevel = auditLevel,
            RetentionDays = retentionDays,
            EnableEmailNotifications = enableEmailNotifications,
            NotificationEmail = notificationEmail,
            LastModified = DateTime.UtcNow,
            ModifiedBy = modifiedBy
        };

        config.RaiseDomainEvent(new AuditConfigurationUpdatedDomainEvent(
            config.Id,
            config.AuditLevel,
            config.RetentionDays,
            config.LastModified));

        return config;
    }

    public void UpdateLevel(AuditLevel newLevel, string modifiedBy)
    {
        AuditLevel = newLevel;
        LastModified = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
        RaiseDomainEvent(new AuditLevelChangedDomainEvent(Id, newLevel, DateTime.UtcNow));
    }

    public void UpdateRetentionPolicy(int retentionDays, string modifiedBy)
    {
        RetentionDays = retentionDays;
        LastModified = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
        RaiseDomainEvent(new AuditRetentionPolicyChangedDomainEvent(Id, retentionDays, DateTime.UtcNow));
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
