namespace Bank.Audit.Infrastructure.Services;

using Bank.Audit.Application.DTOs;
using Bank.Audit.Domain.Enums;
using Bank.Audit.Domain.Repositories;
using Bank.Audit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

internal sealed class AuditQueryService : IAuditQueryService
{
    private readonly AuditDbContext _context;

    public AuditQueryService(AuditDbContext context)
    {
        _context = context;
    }

    public async Task<AuditStatisticsDto> GetStatisticsAsync(CancellationToken cancellationToken)
    {
        var allLogs = await _context.AuditLogs.ToListAsync(cancellationToken);
        var totalEntries = await _context.AuditEntries.CountAsync(cancellationToken);

        return new AuditStatisticsDto(
            TotalLogs: allLogs.Count,
            TotalEntriesRecorded: totalEntries,
            OldestLogDate: allLogs.Any() ? allLogs.Min(a => a.Timestamp) : DateTime.UtcNow,
            NewestLogDate: allLogs.Any() ? allLogs.Max(a => a.Timestamp) : DateTime.UtcNow,
            ActiveLogs: allLogs.Count(a => a.Status == AuditStatus.Active),
            ArchivedLogs: allLogs.Count(a => a.Status == AuditStatus.Archived)
        );
    }
}
