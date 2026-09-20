namespace Bank.Statements.Application.Interfaces;

using Bank.Statements.Domain.Entities;

public interface IStatementLineRepository
{
    Task AddAsync(StatementLine line, CancellationToken ct = default);
    Task AddManyAsync(IEnumerable<StatementLine> lines, CancellationToken ct = default);
    Task<IEnumerable<StatementLine>> GetByStatementIdAsync(Guid statementId, int page = 1, int pageSize = 50, CancellationToken ct = default);
    Task DeleteByStatementIdAsync(Guid statementId, CancellationToken ct = default);
}
