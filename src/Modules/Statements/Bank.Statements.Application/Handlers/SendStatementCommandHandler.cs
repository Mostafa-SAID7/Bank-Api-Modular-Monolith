namespace Bank.Statements.Application.Handlers;

using Bank.Statements.Application.Commands;
using Bank.Statements.Application.Interfaces;

public sealed class SendStatementCommandHandler
{
    private readonly IStatementRepository _repository;

    public SendStatementCommandHandler(IStatementRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(SendStatementCommand command, CancellationToken ct)
    {
        var statement = await _repository.GetByIdAsync(command.StatementId, ct);
        if (statement == null)
            throw new InvalidOperationException($"Statement {command.StatementId} not found");

        statement.Send();
        await _repository.UpdateAsync(statement, ct);
    }
}
