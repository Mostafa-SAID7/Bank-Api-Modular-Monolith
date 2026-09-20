using Bank.Deposits.Application.Commands;
using Bank.Deposits.Application.Interfaces;

namespace Bank.Deposits.Application.Handlers;

/// <summary>
/// Handler for WithdrawFundsCommand
/// </summary>
public class WithdrawFundsCommandHandler : ICommandHandler<WithdrawFundsCommand>
{
    private readonly IDepositRepository _depositRepository;

    public WithdrawFundsCommandHandler(IDepositRepository depositRepository)
    {
        _depositRepository = depositRepository;
    }

    public async Task<Result> Handle(WithdrawFundsCommand command, CancellationToken cancellationToken)
    {
        try
        {
            // Retrieve deposit
            var deposit = await _depositRepository.GetByIdAsync(command.DepositId, cancellationToken);
            if (deposit is null)
                return Result.Failure(new Error("404", "Deposit not found"));

            // Execute domain operation
            deposit.Withdraw(command.Amount, command.Reason);

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
            return Result.Failure(new Error("500", $"Error withdrawing funds: {ex.Message}"));
        }
    }
}
