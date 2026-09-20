namespace Bank.Deposits.Application.Commands;

/// <summary>
/// Command to withdraw funds from a deposit account
/// </summary>
public sealed record WithdrawFundsCommand(
    Guid DepositId,
    decimal Amount,
    string Reason) : Command;
