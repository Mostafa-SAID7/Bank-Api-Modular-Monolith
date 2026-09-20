namespace Bank.Statements.Application.Handlers;

using Bank.Statements.Application.DTOs;
using Bank.Statements.Application.Interfaces;
using Bank.Statements.Application.Queries;

public sealed class GetCustomerStatementsQueryHandler
{
    private readonly IStatementRepository _repository;

    public GetCustomerStatementsQueryHandler(IStatementRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<StatementDto>> Handle(GetCustomerStatementsQuery query, CancellationToken ct)
    {
        var statements = await _repository.GetByCustomerIdAsync(
            query.CustomerId,
            query.Page,
            query.PageSize,
            ct);

        return statements.Select(MapToDto).ToList();
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
