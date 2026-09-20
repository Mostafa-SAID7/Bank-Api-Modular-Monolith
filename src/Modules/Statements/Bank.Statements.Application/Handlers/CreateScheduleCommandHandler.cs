namespace Bank.Statements.Application.Handlers;

using Bank.Statements.Application.Commands;
using Bank.Statements.Application.Interfaces;
using Bank.Statements.Domain.Entities;

public sealed class CreateScheduleCommandHandler
{
    private readonly IStatementScheduleRepository _repository;

    public CreateScheduleCommandHandler(IStatementScheduleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateScheduleCommand command, CancellationToken ct)
    {
        var schedule = StatementSchedule.Create(
            command.CustomerId,
            command.AccountId,
            command.Period,
            command.Format,
            command.DeliveryMethod);

        await _repository.AddAsync(schedule, ct);
        return schedule.Id;
    }
}
