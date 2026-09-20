namespace Bank.Audit.Application.Commands;

public record ArchiveAuditLogCommand(Guid AuditLogId) : IRequest<bool>;
