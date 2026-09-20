namespace Bank.Deposits.Application.Queries;

/// <summary>
/// Query to retrieve a deposit account by ID
/// </summary>
public sealed record GetDepositByIdQuery(Guid DepositId) : Query;
