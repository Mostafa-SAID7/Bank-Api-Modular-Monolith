namespace Bank.Statements.Application.Handlers;

using Bank.Statements.Application.Commands;
using Bank.Statements.Application.Interfaces;

public sealed class CancelStatementCommandHandler
{
    private readonly IStatementRepository _repository;

    public CancelStatementCommandHandler(IStatementRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(CancelStatementCommand command, CancellationToken ct)
    {
        var statement = await _repository.GetByIdAsync(command.StatementId, ct);
        if (statement == null)
            throw new InvalidOperationException($"Statement {command.StatementId} not found");

        statement.Cancel(command.Reason);
        await _repository.UpdateAsync(statement, ct);
    }
}
