namespace Bank.Statements.Infrastructure.Data.Repositories;

using Bank.Statements.Application.Interfaces;
using Bank.Statements.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public sealed class StatementScheduleRepository : IStatementScheduleRepository
{
    private readonly StatementsDbContext _context;

    public StatementScheduleRepository(StatementsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(StatementSchedule schedule, CancellationToken ct = default)
    {
        await _context.StatementSchedules.AddAsync(schedule, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(StatementSchedule schedule, CancellationToken ct = default)
    {
        _context.StatementSchedules.Update(schedule);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<StatementSchedule?> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default)
    {
        return await _context.StatementSchedules
            .FirstOrDefaultAsync(s => s.AccountId == accountId, ct);
    }

    public async Task<StatementSchedule?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.StatementSchedules
            .FirstOrDefaultAsync(s => s.Id == id, ct);
    }

    public async Task<IEnumerable<StatementSchedule>> GetActiveSchedulesAsync(CancellationToken ct = default)
    {
        return await _context.StatementSchedules
            .Where(s => s.IsActive)
            .ToListAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var schedule = await _context.StatementSchedules.FindAsync(new object[] { id }, cancellationToken: ct);
        if (schedule != null)
        {
            _context.StatementSchedules.Remove(schedule);
            await _context.SaveChangesAsync(ct);
        }
    }
}
