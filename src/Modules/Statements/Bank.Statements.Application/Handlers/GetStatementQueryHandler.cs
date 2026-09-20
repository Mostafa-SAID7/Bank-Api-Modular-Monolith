namespace Bank.Statements.Application.Handlers;

using Bank.Statements.Application.DTOs;
using Bank.Statements.Application.Interfaces;
using Bank.Statements.Application.Queries;

public sealed class GetStatementQueryHandler
{
    private readonly IStatementRepository _repository;

    public GetStatementQueryHandler(IStatementRepository repository)
    {
        _repository = repository;
    }

    public async Task<StatementDto?> Handle(GetStatementQuery query, CancellationToken ct)
    {
        var statement = await _repository.GetByIdAsync(query.StatementId, ct);
        if (statement == null)
            return null;

        return MapToDto(statement);
    }

    private static StatementDto MapToDto(Bank.Statements.Domain.Entities.Statement statement)
    {
        return new StatementDto(
            statement.Id,
            statement.CustomerId,
            statement.AccountId,
            statement.StatementDate,
            statement.PeriodStartDate,
            statement.PeriodEndDate,
            statement.OpeningBalance,
            statement.ClosingBalance,
            statement.TotalDebits,
            statement.TotalCredits,
            statement.Status,
            statement.Period,
            statement.PreferredFormat,
            statement.CreatedAt,
            statement.GeneratedAt,
            statement.SentAt,
            statement.DownloadedAt);
    }
}
