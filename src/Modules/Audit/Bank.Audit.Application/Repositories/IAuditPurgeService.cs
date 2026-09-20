namespace Bank.Audit.Domain.Repositories;

public interface IAuditPurgeService
{
    Task<int> PurgeOlderThanAsync(int retentionDays, CancellationToken cancellationToken);
}
