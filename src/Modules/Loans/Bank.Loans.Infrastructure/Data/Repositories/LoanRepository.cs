namespace Bank.Loans.Infrastructure.Data.Repositories;

/// <summary>
/// Repository for Loan aggregate persistence operations
/// Implements ILoanRepository interface from Application layer
/// </summary>
public sealed class LoanRepository : ILoanRepository
{
    private readonly LoansDbContext _context;

    public LoanRepository(LoansDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Loan loan, CancellationToken cancellationToken = default)
    {
        await _context.Loans.AddAsync(loan, cancellationToken);
    }

    public async Task<Loan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Loans
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<Loan?> GetByLoanNumberAsync(string loanNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Loans
            .FirstOrDefaultAsync(l => l.LoanNumber == loanNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<Loan>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Loans
            .Where(l => l.CustomerId == customerId)
            .OrderByDescending(l => l.ApplicationDateUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Loan loan, CancellationToken cancellationToken = default)
    {
        _context.Loans.Update(loan);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var loan = await GetByIdAsync(id, cancellationToken);
        if (loan is not null)
        {
            _context.Loans.Remove(loan);
        }
    }
}
