namespace Bank.Loans.Infrastructure.Data.Repositories;

/// <summary>
/// Repository for LoanProduct persistence operations
/// Implements ILoanProductRepository interface from Application layer
/// </summary>
public sealed class LoanProductRepository : ILoanProductRepository
{
    private readonly LoansDbContext _context;

    public LoanProductRepository(LoansDbContext context)
    {
        _context = context;
    }

    public async Task<LoanProduct?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.LoanProducts
            .FirstOrDefaultAsync(lp => lp.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<LoanProduct>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.LoanProducts
            .Where(lp => true) // All products considered active for now
            .OrderBy(lp => lp.Type)
            .ThenBy(lp => lp.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LoanProduct>> GetByTypeAsync(int loanType, CancellationToken cancellationToken = default)
    {
        return await _context.LoanProducts
            .Where(lp => lp.Type == loanType)
            .OrderBy(lp => lp.Name)
            .ToListAsync(cancellationToken);
    }
}
