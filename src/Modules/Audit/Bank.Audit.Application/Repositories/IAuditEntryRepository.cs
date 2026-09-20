namespace Bank.Audit.Domain.Repositories;

using Bank.Audit.Domain.Entities;

public interface IAuditEntryRepository
{
    Task AddAsync(AuditEntry auditEntry, CancellationToken cancellationToken);
    Task<AuditEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<AuditEntry>> GetByAuditLogIdAsync(Guid auditLogId, CancellationToken cancellationToken);
}
