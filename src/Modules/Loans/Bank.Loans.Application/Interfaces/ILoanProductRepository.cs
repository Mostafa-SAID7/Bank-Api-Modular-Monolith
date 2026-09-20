namespace Bank.Loans.Application.Interfaces;

public interface ILoanProductRepository
{
    Task<LoanProduct?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LoanProduct>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LoanProduct>> GetByTypeAsync(int loanType, CancellationToken cancellationToken = default);
}
