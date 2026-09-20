namespace Bank.Deposits.Application.Queries;

/// <summary>
/// Query to retrieve a fixed deposit account by ID
/// </summary>
public sealed record GetFixedDepositQuery(Guid FixedDepositId) : IRequest<FixedDepositDto>;
