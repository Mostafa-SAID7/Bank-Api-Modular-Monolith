namespace Bank.Audit.Presentation.Responses;

public record GenerateAuditTrailResponse(Guid AuditTrailId, string Message = "Audit trail generated successfully");
