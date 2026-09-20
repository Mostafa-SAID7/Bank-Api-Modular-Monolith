namespace Bank.Audit.Domain.Events;

public sealed record UserActivityRecordedDomainEvent(
    Guid ContextId,
    string UserId,
    DateTime RecordedAt) : DomainEvent(ContextId, "UserActivityRecorded");
