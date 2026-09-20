namespace Bank.Deposits.Application.Commands;

/// <summary>
/// Command to unfreeze a deposit account
/// </summary>
public sealed record UnfreezeDepositCommand(
    Guid DepositId) : Command;
