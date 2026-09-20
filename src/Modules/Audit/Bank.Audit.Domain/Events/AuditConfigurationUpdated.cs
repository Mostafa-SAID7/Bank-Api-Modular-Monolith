namespace Bank.Audit.Domain.Events;

using Bank.Audit.Domain.Enums;

public sealed record AuditConfigurationUpdatedDomainEvent(
    Guid ConfigurationId,
    AuditLevel AuditLevel,
    int RetentionDays,
    DateTime UpdatedAt) : DomainEvent(ConfigurationId, "AuditConfigurationUpdated");
