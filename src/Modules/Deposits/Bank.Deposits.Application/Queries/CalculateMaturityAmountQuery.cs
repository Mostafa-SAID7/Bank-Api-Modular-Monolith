namespace Bank.Deposits.Application.Queries;

/// <summary>
/// Query to calculate maturity amount for a fixed deposit
/// </summary>
public sealed record CalculateMaturityAmountQuery(
    Guid DepositId,
    decimal Principal,
    decimal AnnualRate,
    int TermMonths,
    InterestCalculationMethod CalculationMethod) : Query;
