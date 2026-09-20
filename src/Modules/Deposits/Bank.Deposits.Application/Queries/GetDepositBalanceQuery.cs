namespace Bank.Deposits.Application.Queries;

/// <summary>
/// Query to retrieve current balance information for a deposit account
/// </summary>
public sealed record GetDepositBalanceQuery(Guid DepositId) : Query;
