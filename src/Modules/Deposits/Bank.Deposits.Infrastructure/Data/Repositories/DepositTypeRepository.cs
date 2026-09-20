namespace Bank.Deposits.Infrastructure.Data.Repositories;

/// <summary>
/// Repository for DepositType entity persistence operations
/// Implements IDepositTypeRepository interface from Application layer
/// </summary>
public sealed class DepositTypeRepository : IDepositTypeRepository
{
    private readonly DepositsDbContext _context;

    public DepositTypeRepository(DepositsDbContext context)
    {
        _context = context;
    }

    public async Task<DepositType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.DepositTypes
            .FirstOrDefaultAsync(dt => dt.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<DepositType>> GetActiveTypesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DepositTypes
            .Where(dt => dt.IsActive)
            .ToListAsync(cancellationToken);
    }
}
