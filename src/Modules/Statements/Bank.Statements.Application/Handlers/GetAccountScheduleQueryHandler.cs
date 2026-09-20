namespace Bank.Statements.Application.Handlers;

using Bank.Statements.Application.DTOs;
using Bank.Statements.Application.Interfaces;
using Bank.Statements.Application.Queries;

public sealed class GetAccountScheduleQueryHandler
{
    private readonly IStatementScheduleRepository _repository;

    public GetAccountScheduleQueryHandler(IStatementScheduleRepository repository)
    {
        _repository = repository;
    }

    public async Task<StatementScheduleDto?> Handle(GetAccountScheduleQuery query, CancellationToken ct)
    {
        var schedule = await _repository.GetByAccountIdAsync(query.AccountId, ct);
        if (schedule == null)
            return null;

        return MapToDto(schedule);
    }

    private static StatementScheduleDto MapToDto(Bank.Statements.Domain.Entities.StatementSchedule schedule)
    {
        return new StatementScheduleDto(
            schedule.Id,
            schedule.CustomerId,
            schedule.AccountId,
            schedule.Period,
            schedule.PreferredFormat,
            schedule.DeliveryMethod,
            schedule.IsActive,
            schedule.NextGenerationDate,
            schedule.LastGeneratedDate,
            schedule.CreatedAt);
    }
}
