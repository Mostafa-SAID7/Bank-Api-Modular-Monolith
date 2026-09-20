namespace Bank.Audit.Domain.Repositories;

using Bank.Audit.Domain.Entities;

public interface IAuditTrailRepository
{
    Task AddAsync(AuditTrail auditTrail, CancellationToken cancellationToken);
    Task<AuditTrail?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
