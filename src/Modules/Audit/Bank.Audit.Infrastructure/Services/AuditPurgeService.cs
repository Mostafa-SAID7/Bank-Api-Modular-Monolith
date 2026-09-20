namespace Bank.Audit.Infrastructure.Services;

using Bank.Audit.Domain.Entities;
using Bank.Audit.Domain.Enums;
using Bank.Audit.Domain.Repositories;
using Bank.Audit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

internal sealed class AuditPurgeService : IAuditPurgeService
{
    private readonly AuditDbContext _context;

    public AuditPurgeService(AuditDbContext context)
    {
        _context = context;
    }

    public async Task<int> PurgeOlderThanAsync(int retentionDays, CancellationToken cancellationToken)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);
        var logsToDelete = await _context.AuditLogs
            .Where(a => a.Timestamp < cutoffDate && a.Status != AuditStatus.Archived)
            .ToListAsync(cancellationToken);

        if (logsToDelete.Count == 0)
            return 0;

        _context.AuditLogs.RemoveRange(logsToDelete);
        await _context.SaveChangesAsync(cancellationToken);
        return logsToDelete.Count;
    }
}
