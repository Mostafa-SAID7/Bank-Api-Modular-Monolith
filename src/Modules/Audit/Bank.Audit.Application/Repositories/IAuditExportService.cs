namespace Bank.Audit.Domain.Repositories;

public interface IAuditExportService
{
    Task<string> ExportAuditLogAsync(Guid auditLogId, string exportFormat, CancellationToken cancellationToken);
}
