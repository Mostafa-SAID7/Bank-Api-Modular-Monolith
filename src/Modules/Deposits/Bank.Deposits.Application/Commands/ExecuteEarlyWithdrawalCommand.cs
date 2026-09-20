namespace Bank.Deposits.Application.Commands;

/// <summary>
/// Command to execute early withdrawal from a fixed deposit
/// </summary>
public sealed record ExecuteEarlyWithdrawalCommand(
    Guid FixedDepositId,
    decimal WithdrawalAmount) : IRequest<FixedDeposit>;
