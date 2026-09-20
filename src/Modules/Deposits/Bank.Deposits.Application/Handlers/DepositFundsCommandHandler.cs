using Bank.Deposits.Application.Commands;
using Bank.Deposits.Application.Interfaces;

namespace Bank.Deposits.Application.Handlers;

/// <summary>
/// Handler for DepositFundsCommand
/// </summary>
public class DepositFundsCommandHandler : ICommandHandler<DepositFundsCommand>
{
    private readonly IDepositRepository _depositRepository;

    public DepositFundsCommandHandler(IDepositRepository depositRepository)
    {
        _depositRepository = depositRepository;
    }

    public async Task<Result> Handle(DepositFundsCommand command, CancellationToken cancellationToken)
    {
        try
        {
            // Retrieve deposit
            var deposit = await _depositRepository.GetByIdAsync(command.DepositId, cancellationToken);
            if (deposit is null)
                return Result.Failure(new Error("404", "Deposit not found"));

            // Execute domain operation
            deposit.Deposit(command.Amount, command.Description);

            // Persist changes
            await _depositRepository.UpdateAsync(deposit, cancellationToken);

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(new Error("400", ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", $"Error depositing funds: {ex.Message}"));
        }
    }
}
