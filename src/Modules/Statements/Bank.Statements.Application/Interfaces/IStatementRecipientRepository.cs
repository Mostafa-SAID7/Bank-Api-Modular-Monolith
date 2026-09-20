namespace Bank.Statements.Application.Interfaces;

using Bank.Statements.Domain.Entities;

public interface IStatementRecipientRepository
{
    Task AddAsync(StatementRecipient recipient, CancellationToken ct = default);
    Task<IEnumerable<StatementRecipient>> GetByStatementIdAsync(Guid statementId, CancellationToken ct = default);
    Task<StatementRecipient?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task UpdateAsync(StatementRecipient recipient, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
