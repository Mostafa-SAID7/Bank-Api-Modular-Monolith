namespace Bank.Audit.Domain.Repositories;

using Bank.Audit.Application.DTOs;

public interface IAuditQueryService
{
    Task<AuditStatisticsDto> GetStatisticsAsync(CancellationToken cancellationToken);
}
