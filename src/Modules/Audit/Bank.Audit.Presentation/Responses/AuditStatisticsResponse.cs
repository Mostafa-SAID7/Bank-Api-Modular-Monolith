namespace Bank.Audit.Presentation.Responses;

public record AuditStatisticsResponse(
    int TotalLogs,
    int TotalEntriesRecorded,
    DateTime OldestLogDate,
    DateTime NewestLogDate,
    int ActiveLogs,
    int ArchivedLogs);
