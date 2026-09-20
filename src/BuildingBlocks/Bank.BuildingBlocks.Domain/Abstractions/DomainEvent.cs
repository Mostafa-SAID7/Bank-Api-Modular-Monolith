namespace Bank.BuildingBlocks.Domain.Abstractions;

/// <summary>
/// Base domain event for all aggregate root events
/// Provides common tracking of event metadata (ID, timestamp, aggregate ID)
/// </summary>
public abstract record DomainEvent(Guid AggregateId, string EventType)
{
    /// <summary>
    /// Unique identifier for this event
    /// </summary>
    public Guid EventId { get; } = Guid.NewGuid();

    /// <summary>
    /// Timestamp when the event occurred
    /// </summary>
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
