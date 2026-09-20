namespace Bank.Audit.Domain.Entities;

using Bank.Audit.Domain.Events;

public class AuditTrail
{
    public Guid Id { get; private set; }
    public string TrailName { get; private set; } = string.Empty;
    public DateTime GeneratedAt { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int TotalEntries { get; private set; }
    public string FilePath { get; private set; } = string.Empty;
    public string FileFormat { get; private set; } = string.Empty;

    private readonly List<object> _domainEvents = new();

    public IReadOnlyList<object> DomainEvents => _domainEvents.AsReadOnly();

    private AuditTrail() { }

    public static AuditTrail Create(
        string trailName,
        DateTime startDate,
        DateTime endDate,
        int totalEntries,
        string filePath,
        string fileFormat)
    {
        var trail = new AuditTrail
        {
            Id = Guid.NewGuid(),
            TrailName = trailName,
            GeneratedAt = DateTime.UtcNow,
            StartDate = startDate,
            EndDate = endDate,
            TotalEntries = totalEntries,
            FilePath = filePath,
            FileFormat = fileFormat
        };

        trail.RaiseDomainEvent(new AuditTrailGeneratedDomainEvent(
            trail.Id,
            trail.TrailName,
            trail.GeneratedAt,
            trail.TotalEntries));

        return trail;
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
