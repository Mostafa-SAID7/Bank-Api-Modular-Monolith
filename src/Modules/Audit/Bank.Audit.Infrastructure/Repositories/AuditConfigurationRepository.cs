namespace Bank.Audit.Infrastructure.Repositories;

using Bank.Audit.Domain.Entities;
using Bank.Audit.Domain.Repositories;
using Bank.Audit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

internal sealed class AuditConfigurationRepository : IAuditConfigurationRepository
{
    private readonly AuditDbContext _context;

    public AuditConfigurationRepository(AuditDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AuditConfiguration configuration, CancellationToken cancellationToken)
    {
        await _context.AuditConfigurations.AddAsync(configuration, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<AuditConfiguration?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.AuditConfigurations.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task UpdateAsync(AuditConfiguration configuration, CancellationToken cancellationToken)
    {
        _context.AuditConfigurations.Update(configuration);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
