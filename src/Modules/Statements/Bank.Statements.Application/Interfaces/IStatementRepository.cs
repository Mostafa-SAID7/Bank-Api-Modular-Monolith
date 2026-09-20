namespace Bank.Statements.Application.Interfaces;

using Bank.Statements.Domain.Entities;

public interface IStatementRepository
{
    Task AddAsync(Statement statement, CancellationToken ct = default);
    Task UpdateAsync(Statement statement, CancellationToken ct = default);
    Task<Statement?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Statement>> GetByCustomerIdAsync(Guid customerId, int page = 1, int pageSize = 20, CancellationToken ct = default);
    Task<IEnumerable<Statement>> GetByAccountIdAsync(Guid accountId, int page = 1, int pageSize = 20, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
