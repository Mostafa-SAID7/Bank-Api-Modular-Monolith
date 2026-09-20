namespace Bank.Statements.Infrastructure.Data.Repositories;

using Bank.Statements.Application.Interfaces;
using Bank.Statements.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public sealed class StatementRepository : IStatementRepository
{
    private readonly StatementsDbContext _context;

    public StatementRepository(StatementsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Statement statement, CancellationToken ct = default)
    {
        await _context.Statements.AddAsync(statement, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Statement statement, CancellationToken ct = default)
    {
        _context.Statements.Update(statement);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Statement?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Statements
            .Include(s => s.Lines)
            .Include(s => s.Recipients)
            .FirstOrDefaultAsync(s => s.Id == id, ct);
    }

    public async Task<IEnumerable<Statement>> GetByCustomerIdAsync(Guid customerId, int page = 1, int pageSize = 20, CancellationToken ct = default)
    {
        return await _context.Statements
            .Where(s => s.CustomerId == customerId)
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Statement>> GetByAccountIdAsync(Guid accountId, int page = 1, int pageSize = 20, CancellationToken ct = default)
    {
        return await _context.Statements
            .Where(s => s.AccountId == accountId)
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var statement = await _context.Statements.FindAsync(new object[] { id }, cancellationToken: ct);
        if (statement != null)
        {
            _context.Statements.Remove(statement);
            await _context.SaveChangesAsync(ct);
        }
    }
}
