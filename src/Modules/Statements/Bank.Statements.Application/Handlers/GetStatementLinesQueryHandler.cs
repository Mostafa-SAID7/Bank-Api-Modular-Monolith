namespace Bank.Statements.Application.Handlers;

using Bank.Statements.Application.DTOs;
using Bank.Statements.Application.Interfaces;
using Bank.Statements.Application.Queries;

public sealed class GetStatementLinesQueryHandler
{
    private readonly IStatementLineRepository _repository;

    public GetStatementLinesQueryHandler(IStatementLineRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<StatementLineDto>> Handle(GetStatementLinesQuery query, CancellationToken ct)
    {
        var lines = await _repository.GetByStatementIdAsync(
            query.StatementId,
            query.Page,
            query.PageSize,
            ct);

        return lines.Select(MapToDto).ToList();
    }

    private static StatementLineDto MapToDto(Bank.Statements.Domain.Entities.StatementLine line)
    {
        return new StatementLineDto(
            line.Id,
            line.TransactionDate,
            line.PostedDate,
            line.Description,
            line.LineType,
            line.Amount,
            line.Balance,
            line.Reference,
            line.ChequeNumber);
    }
}
