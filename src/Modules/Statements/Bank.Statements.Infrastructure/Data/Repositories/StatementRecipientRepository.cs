namespace Bank.Statements.Infrastructure.Data.Repositories;

using Bank.Statements.Application.Interfaces;
using Bank.Statements.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public sealed class StatementRecipientRepository : IStatementRecipientRepository
{
    private readonly StatementsDbContext _context;

    public StatementRecipientRepository(StatementsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(StatementRecipient recipient, CancellationToken ct = default)
    {
        await _context.StatementRecipients.AddAsync(recipient, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<StatementRecipient>> GetByStatementIdAsync(Guid statementId, CancellationToken ct = default)
    {
        return await _context.StatementRecipients
            .Where(r => r.StatementId == statementId)
            .ToListAsync(ct);
    }

    public async Task<StatementRecipient?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.StatementRecipients
            .FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    public async Task UpdateAsync(StatementRecipient recipient, CancellationToken ct = default)
    {
        _context.StatementRecipients.Update(recipient);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var recipient = await _context.StatementRecipients.FindAsync(new object[] { id }, cancellationToken: ct);
        if (recipient != null)
        {
            _context.StatementRecipients.Remove(recipient);
            await _context.SaveChangesAsync(ct);
        }
    }
}
