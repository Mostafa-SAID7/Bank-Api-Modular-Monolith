namespace Bank.Audit.Domain.Events;

public sealed record AuditRetentionPolicyChangedDomainEvent(
    Guid ConfigurationId,
    int RetentionDays,
    DateTime ChangedAt) : DomainEvent(ConfigurationId, "AuditRetentionPolicyChanged");
