namespace Bank.Audit.Presentation.Responses;

public record ExportAuditLogResponse(string ExportPath, string Message = "Audit log exported successfully");
