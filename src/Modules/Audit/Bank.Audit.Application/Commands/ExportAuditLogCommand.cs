namespace Bank.Audit.Application.Commands;

public record ExportAuditLogCommand(Guid AuditLogId, string ExportFormat) : IRequest<string>;
