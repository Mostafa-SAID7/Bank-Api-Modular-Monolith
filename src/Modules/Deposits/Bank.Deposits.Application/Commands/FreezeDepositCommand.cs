namespace Bank.Deposits.Application.Commands;

/// <summary>
/// Command to freeze a deposit account (block withdrawals)
/// </summary>
public sealed record FreezeDepositCommand(
    Guid DepositId,
    string Reason) : Command;
