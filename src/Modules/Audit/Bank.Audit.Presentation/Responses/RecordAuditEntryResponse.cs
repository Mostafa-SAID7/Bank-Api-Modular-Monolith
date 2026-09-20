namespace Bank.Audit.Presentation.Responses;

public record RecordAuditEntryResponse(Guid AuditEntryId, string Message = "Audit entry recorded successfully");
