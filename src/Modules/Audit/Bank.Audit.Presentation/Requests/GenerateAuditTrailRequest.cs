namespace Bank.Audit.Presentation.Requests;

public record GenerateAuditTrailRequest(
    string TrailName,
    DateTime StartDate,
    DateTime EndDate,
    int TotalEntries,
    string FilePath,
    string FileFormat);
