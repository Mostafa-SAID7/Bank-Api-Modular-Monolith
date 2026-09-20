namespace Bank.Deposits.Application.Commands;

/// <summary>
/// Command to pay accrued interest to a deposit account
/// </summary>
public sealed record PayDepositInterestCommand(
    Guid DepositId) : Command;
