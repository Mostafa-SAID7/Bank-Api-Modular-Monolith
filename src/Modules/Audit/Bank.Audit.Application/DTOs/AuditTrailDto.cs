namespace Bank.Audit.Application.DTOs;

public record AuditTrailDto(
    Guid Id,
    string TrailName,
    DateTime GeneratedAt,
    DateTime StartDate,
    DateTime EndDate,
    int TotalEntries,
    string FilePath,
    string FileFormat);
