namespace Bank.Loans.Application.Interfaces;

public interface ILoanRepository
{
    Task AddAsync(Loan loan, CancellationToken cancellationToken = default);
    Task<Loan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Loan?> GetByLoanNumberAsync(string loanNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Loan>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Loan loan, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
