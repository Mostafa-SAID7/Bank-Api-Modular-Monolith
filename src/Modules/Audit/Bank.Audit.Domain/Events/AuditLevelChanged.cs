namespace Bank.Audit.Domain.Events;

using Bank.Audit.Domain.Enums;

public sealed record AuditLevelChangedDomainEvent(
    Guid ConfigurationId,
    AuditLevel NewLevel,
    DateTime ChangedAt) : DomainEvent(ConfigurationId, "AuditLevelChanged");
