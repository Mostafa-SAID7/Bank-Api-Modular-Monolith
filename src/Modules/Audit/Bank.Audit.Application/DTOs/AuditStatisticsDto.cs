namespace Bank.Audit.Application.DTOs;

public record AuditStatisticsDto(
    int TotalLogs,
    int TotalEntriesRecorded,
    DateTime OldestLogDate,
    DateTime NewestLogDate,
    int ActiveLogs,
    int ArchivedLogs);
