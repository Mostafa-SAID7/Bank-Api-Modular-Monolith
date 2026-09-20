namespace Bank.Audit.Domain.Repositories;

using Bank.Audit.Domain.Entities;

public interface IAuditConfigurationRepository
{
    Task AddAsync(AuditConfiguration configuration, CancellationToken cancellationToken);
    Task<AuditConfiguration?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateAsync(AuditConfiguration configuration, CancellationToken cancellationToken);
}
