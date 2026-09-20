namespace Bank.Statements.Application.Handlers;

using Bank.Statements.Application.Commands;
using Bank.Statements.Application.Interfaces;

public sealed class UpdateScheduleCommandHandler
{
    private readonly IStatementScheduleRepository _repository;

    public UpdateScheduleCommandHandler(IStatementScheduleRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(UpdateScheduleCommand command, CancellationToken ct)
    {
        var schedule = await _repository.GetByIdAsync(command.ScheduleId, ct);
        if (schedule == null)
            throw new InvalidOperationException($"Schedule {command.ScheduleId} not found");

        schedule.UpdateSchedule(command.Period, command.Format, command.DeliveryMethod);
        await _repository.UpdateAsync(schedule, ct);
    }
}
