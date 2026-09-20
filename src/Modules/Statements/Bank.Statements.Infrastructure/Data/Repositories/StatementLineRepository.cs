namespace Bank.Statements.Infrastructure.Data.Repositories;

using Bank.Statements.Application.Interfaces;
using Bank.Statements.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public sealed class StatementLineRepository : IStatementLineRepository
{
    private readonly StatementsDbContext _context;

    public StatementLineRepository(StatementsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(StatementLine line, CancellationToken ct = default)
    {
        await _context.StatementLines.AddAsync(line, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task AddManyAsync(IEnumerable<StatementLine> lines, CancellationToken ct = default)
    {
        await _context.StatementLines.AddRangeAsync(lines, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<StatementLine>> GetByStatementIdAsync(Guid statementId, int page = 1, int pageSize = 50, CancellationToken ct = default)
    {
        return await _context.StatementLines
            .Where(l => l.StatementId == statementId)
            .OrderBy(l => l.TransactionDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task DeleteByStatementIdAsync(Guid statementId, CancellationToken ct = default)
    {
        var lines = await _context.StatementLines
            .Where(l => l.StatementId == statementId)
            .ToListAsync(ct);

        _context.StatementLines.RemoveRange(lines);
        await _context.SaveChangesAsync(ct);
    }
}
