namespace Bank.Statements.Application.Handlers;

using Bank.Statements.Application.Commands;
using Bank.Statements.Application.Interfaces;

public sealed class ArchiveStatementCommandHandler
{
    private readonly IStatementRepository _repository;

    public ArchiveStatementCommandHandler(IStatementRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(ArchiveStatementCommand command, CancellationToken ct)
    {
        var statement = await _repository.GetByIdAsync(command.StatementId, ct);
        if (statement == null)
            throw new InvalidOperationException($"Statement {command.StatementId} not found");

        statement.Archive(command.Reason, command.Notes);
        await _repository.UpdateAsync(statement, ct);
    }
}
