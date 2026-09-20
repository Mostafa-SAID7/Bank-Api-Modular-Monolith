namespace Bank.Deposits.Application.Commands;

/// <summary>
/// Command to open a new deposit account
/// </summary>
public sealed record OpenDepositCommand(
    Guid CustomerId,
    Guid DepositTypeId,
    decimal InitialDeposit,
    bool IsFixedDeposit,
    int TermMonths) : Command;
