namespace Bank.Audit.Presentation.Responses;

public record CreateAuditLogResponse(Guid AuditLogId, string Message = "Audit log created successfully");
