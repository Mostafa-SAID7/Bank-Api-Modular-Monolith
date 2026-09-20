namespace Bank.Audit.Infrastructure.Repositories;

using Bank.Audit.Domain.Entities;
using Bank.Audit.Domain.Repositories;
using Bank.Audit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

internal sealed class AuditTrailRepository : IAuditTrailRepository
{
    private readonly AuditDbContext _context;

    public AuditTrailRepository(AuditDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AuditTrail auditTrail, CancellationToken cancellationToken)
    {
        await _context.AuditTrails.AddAsync(auditTrail, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<AuditTrail?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.AuditTrails.FindAsync(new object[] { id }, cancellationToken);
    }
}
