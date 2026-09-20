namespace Bank.Deposits.Application.Commands;

/// <summary>
/// Command to deposit funds into a deposit account
/// </summary>
public sealed record DepositFundsCommand(
    Guid DepositId,
    decimal Amount,
    string Description) : Command;
