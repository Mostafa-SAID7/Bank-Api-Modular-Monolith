using Bank.Deposits.Application.Commands;
using Bank.Deposits.Application.Interfaces;

namespace Bank.Deposits.Application.Handlers;

/// <summary>
/// Handler for UnfreezeDepositCommand
/// </summary>
public class UnfreezeDepositCommandHandler : ICommandHandler<UnfreezeDepositCommand>
{
    private readonly IDepositRepository _depositRepository;

    public UnfreezeDepositCommandHandler(IDepositRepository depositRepository)
    {
        _depositRepository = depositRepository;
    }

    public async Task<Result> Handle(UnfreezeDepositCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var deposit = await _depositRepository.GetByIdAsync(command.DepositId, cancellationToken);
            if (deposit is null)
                return Result.Failure(new Error("404", "Deposit not found"));

            deposit.Unfreeze();
            await _depositRepository.UpdateAsync(deposit, cancellationToken);

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(new Error("400", ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", $"Error unfreezing deposit: {ex.Message}"));
        }
    }
}
