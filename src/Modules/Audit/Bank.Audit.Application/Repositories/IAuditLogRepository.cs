namespace Bank.Audit.Domain.Repositories;

using Bank.Audit.Domain.Entities;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken);
    Task<AuditLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<AuditLog>> GetByUserIdAsync(string userId, CancellationToken cancellationToken);
    Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
}
