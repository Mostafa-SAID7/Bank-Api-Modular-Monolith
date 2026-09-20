namespace Bank.Statements.Application.Handlers;

using Bank.Statements.Application.Commands;
using Bank.Statements.Application.Interfaces;

public sealed class DownloadStatementCommandHandler
{
    private readonly IStatementRepository _repository;

    public DownloadStatementCommandHandler(IStatementRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DownloadStatementCommand command, CancellationToken ct)
    {
        var statement = await _repository.GetByIdAsync(command.StatementId, ct);
        if (statement == null)
            throw new InvalidOperationException($"Statement {command.StatementId} not found");

        if (statement.CustomerId != command.CustomerId)
            throw new UnauthorizedAccessException("Customer is not authorized to download this statement");

        statement.MarkAsDownloaded();
        await _repository.UpdateAsync(statement, ct);
    }
}
