namespace Bank.Deposits.Application.Queries;

/// <summary>
/// Query to retrieve the maturity schedule for a fixed deposit
/// </summary>
public sealed record GetMaturityScheduleQuery(Guid FixedDepositId) : IRequest<MaturityScheduleDto>;
