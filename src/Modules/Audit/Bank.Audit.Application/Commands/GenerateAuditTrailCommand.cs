namespace Bank.Audit.Application.Commands;

public record GenerateAuditTrailCommand(
    string TrailName,
    DateTime StartDate,
    DateTime EndDate,
    int TotalEntries,
    string FilePath,
    string FileFormat) : IRequest<Guid>;
