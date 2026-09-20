namespace Bank.Deposits.Infrastructure.Data.Repositories;

/// <summary>
/// Repository for Deposit aggregate persistence operations
/// Implements IDepositRepository interface from Application layer
/// </summary>
public sealed class DepositRepository : IDepositRepository
{
    private readonly DepositsDbContext _context;

    public DepositRepository(DepositsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Deposit deposit, CancellationToken cancellationToken = default)
    {
        await _context.Deposits.AddAsync(deposit, cancellationToken);
    }

    public async Task<Deposit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Deposits
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<Deposit?> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Deposits
            .FirstOrDefaultAsync(d => d.AccountNumber == accountNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<Deposit>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Deposits
            .Where(d => d.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Deposit deposit, CancellationToken cancellationToken = default)
    {
        _context.Deposits.Update(deposit);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var deposit = await GetByIdAsync(id, cancellationToken);
        if (deposit is not null)
        {
            _context.Deposits.Remove(deposit);
        }
    }
}
