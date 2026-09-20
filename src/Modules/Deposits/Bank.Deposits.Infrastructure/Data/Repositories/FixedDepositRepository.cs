namespace Bank.Deposits.Infrastructure.Data.Repositories;

/// <summary>
/// Repository for FixedDeposit aggregate persistence operations
/// Implements IFixedDepositRepository interface from Application layer
/// </summary>
public sealed class FixedDepositRepository : IFixedDepositRepository
{
    private readonly DepositsDbContext _context;

    public FixedDepositRepository(DepositsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(FixedDeposit fixedDeposit, CancellationToken cancellationToken = default)
    {
        await _context.FixedDeposits.AddAsync(fixedDeposit, cancellationToken);
    }

    public async Task<FixedDeposit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.FixedDeposits
            .FirstOrDefaultAsync(fd => fd.Id == id, cancellationToken);
    }

    public async Task<FixedDeposit?> GetByFixedDepositNumberAsync(string fixedDepositNumber, CancellationToken cancellationToken = default)
    {
        return await _context.FixedDeposits
            .FirstOrDefaultAsync(fd => fd.DepositNumber == fixedDepositNumber, cancellationToken);
    }

    public async Task<List<FixedDeposit>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.FixedDeposits
            .Where(fd => fd.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<FixedDeposit>> GetFixedDepositsReadyForMaturityAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.FixedDeposits
            .Where(fd => fd.Status == FixedDepositStatus.Active && fd.MaturityDateUtc <= now)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(FixedDeposit fixedDeposit, CancellationToken cancellationToken = default)
    {
        _context.FixedDeposits.Update(fixedDeposit);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var fixedDeposit = await GetByIdAsync(id, cancellationToken);
        if (fixedDeposit is not null)
        {
            _context.FixedDeposits.Remove(fixedDeposit);
        }
    }
}
