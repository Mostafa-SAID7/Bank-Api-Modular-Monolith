namespace Bank.Audit.Infrastructure.Repositories;

using Bank.Audit.Domain.Entities;
using Bank.Audit.Domain.Repositories;
using Bank.Audit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

internal sealed class AuditLogRepository : IAuditLogRepository
{
    private readonly AuditDbContext _context;

    public AuditLogRepository(AuditDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken)
    {
        await _context.AuditLogs.AddAsync(auditLog, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<AuditLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.AuditLogs.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await _context.AuditLogs
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        return await _context.AuditLogs
            .Where(a => a.Timestamp >= startDate && a.Timestamp <= endDate)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(cancellationToken);
    }
}
