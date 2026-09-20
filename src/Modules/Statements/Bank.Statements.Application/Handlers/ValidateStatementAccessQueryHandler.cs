namespace Bank.Statements.Application.Handlers;

using Bank.Statements.Application.Interfaces;
using Bank.Statements.Application.Queries;

public sealed class ValidateStatementAccessQueryHandler
{
    private readonly IStatementRepository _repository;

    public ValidateStatementAccessQueryHandler(IStatementRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(ValidateStatementAccessQuery query, CancellationToken ct)
    {
        var statement = await _repository.GetByIdAsync(query.StatementId, ct);
        if (statement == null)
            return false;

        return statement.CustomerId == query.CustomerId && statement.IsAccessible;
    }
}
