namespace Bank.Audit.Infrastructure.Repositories;

using Bank.Audit.Domain.Entities;
using Bank.Audit.Domain.Repositories;
using Bank.Audit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

internal sealed class AuditEntryRepository : IAuditEntryRepository
{
    private readonly AuditDbContext _context;

    public AuditEntryRepository(AuditDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AuditEntry auditEntry, CancellationToken cancellationToken)
    {
        await _context.AuditEntries.AddAsync(auditEntry, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<AuditEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.AuditEntries.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<AuditEntry>> GetByAuditLogIdAsync(Guid auditLogId, CancellationToken cancellationToken)
    {
        return await _context.AuditEntries
            .Where(a => a.AuditLogId == auditLogId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
