namespace Bank.Audit.Presentation.Requests;

public record RecordAuditEntryRequest(
    Guid AuditLogId,
    string EntityName,
    string ActionType,
    string OldValues,
    string NewValues,
    string ChangeType);
