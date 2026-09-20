namespace Bank.Audit.Infrastructure.Services;

using Bank.Audit.Domain.Repositories;
using Bank.Audit.Infrastructure.Data;

internal sealed class AuditExportService : IAuditExportService
{
    private readonly AuditDbContext _context;

    public AuditExportService(AuditDbContext context)
    {
        _context = context;
    }

    public async Task<string> ExportAuditLogAsync(Guid auditLogId, string exportFormat, CancellationToken cancellationToken)
    {
        var auditLog = await _context.AuditLogs.FindAsync(new object[] { auditLogId }, cancellationToken);
        if (auditLog is null)
            throw new InvalidOperationException($"Audit log {auditLogId} not found");

        var exportPath = $"/exports/audit_{auditLogId}_{DateTime.UtcNow:yyyyMMddHHmmss}.{exportFormat}";
        // Placeholder for actual export logic
        return exportPath;
    }
}
