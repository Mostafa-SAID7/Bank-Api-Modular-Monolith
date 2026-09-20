namespace Bank.Deposits.Application.Commands;

/// <summary>
/// Command to close a deposit account
/// </summary>
public sealed record CloseDepositCommand(
    Guid DepositId,
    string Reason) : Command;
