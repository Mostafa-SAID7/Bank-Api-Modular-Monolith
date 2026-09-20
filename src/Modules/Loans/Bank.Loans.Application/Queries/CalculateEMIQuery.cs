namespace Bank.Loans.Application.Queries;

public record CalculateEMIQuery(
    decimal Principal,
    decimal AnnualRate,
    int TenureMonths) : IRequest<EMICalculationDto>;
