namespace Bank.Audit.Domain.Events;

public sealed record AuditTrailGeneratedDomainEvent(
    Guid AuditTrailId,
    string TrailName,
    DateTime GeneratedAt,
    int TotalEntries) : DomainEvent(AuditTrailId, "AuditTrailGenerated");
