namespace Bank.Statements.Application.Interfaces;

using Bank.Statements.Domain.Entities;

public interface IStatementScheduleRepository
{
    Task AddAsync(StatementSchedule schedule, CancellationToken ct = default);
    Task UpdateAsync(StatementSchedule schedule, CancellationToken ct = default);
    Task<StatementSchedule?> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default);
    Task<StatementSchedule?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<StatementSchedule>> GetActiveSchedulesAsync(CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
