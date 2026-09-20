namespace Bank.Deposits.Infrastructure.Data.Repositories;

/// <summary>
/// Repository for DepositProduct persistence operations
/// Implements IDepositProductRepository interface from Application layer
/// </summary>
public sealed class DepositProductRepository : IDepositProductRepository
{
    private readonly DepositsDbContext _context;

    public DepositProductRepository(DepositsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(DepositProduct depositProduct, CancellationToken cancellationToken = default)
    {
        await _context.DepositProducts.AddAsync(depositProduct, cancellationToken);
    }

    public async Task<DepositProduct?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.DepositProducts
            .FirstOrDefaultAsync(dp => dp.Id == id, cancellationToken);
    }

    public async Task<List<DepositProduct>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DepositProducts
            .Where(dp => dp.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<DepositProduct>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DepositProducts
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(DepositProduct depositProduct, CancellationToken cancellationToken = default)
    {
        _context.DepositProducts.Update(depositProduct);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var depositProduct = await GetByIdAsync(id, cancellationToken);
        if (depositProduct is not null)
        {
            _context.DepositProducts.Remove(depositProduct);
        }
    }
}
