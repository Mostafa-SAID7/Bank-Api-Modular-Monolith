namespace Bank.Deposits.Application.Handlers;

/// <summary>
/// Handler for ExecuteEarlyWithdrawalCommand
/// </summary>
public class ExecuteEarlyWithdrawalCommandHandler : IRequestHandler<ExecuteEarlyWithdrawalCommand, FixedDeposit>
{
    private readonly IFixedDepositRepository _fixedDepositRepository;

    public ExecuteEarlyWithdrawalCommandHandler(IFixedDepositRepository fixedDepositRepository)
    {
        _fixedDepositRepository = fixedDepositRepository;
    }

    public async Task<FixedDeposit> Handle(ExecuteEarlyWithdrawalCommand request, CancellationToken cancellationToken)
    {
        var fixedDeposit = await _fixedDepositRepository.GetByIdAsync(request.FixedDepositId, cancellationToken);
        if (fixedDeposit == null)
            throw new InvalidOperationException("Fixed deposit not found");

        if (fixedDeposit.Status != FixedDepositStatus.Active)
            throw new InvalidOperationException("Early withdrawal is only allowed for active deposits");

        if (request.WithdrawalAmount <= 0)
            throw new InvalidOperationException("Withdrawal amount must be greater than zero");

        if (request.WithdrawalAmount > fixedDeposit.PrincipalAmount)
            throw new InvalidOperationException("Withdrawal amount cannot exceed principal");

        var penalty = fixedDeposit.CalculateEarlyWithdrawalPenalty(request.WithdrawalAmount);

        // Note: Domain model doesn't expose method to record transactions directly
        // This would require an extension to the FixedDeposit aggregate
        // For now, just update the repository

        await _fixedDepositRepository.UpdateAsync(fixedDeposit, cancellationToken);
        return fixedDeposit;
    }
}
