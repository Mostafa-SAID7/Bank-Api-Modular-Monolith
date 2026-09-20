namespace Bank.Audit.Presentation.Responses;

public record GetAuditLogsResponse(IEnumerable<GetAuditLogResponse> Logs, int Count);
