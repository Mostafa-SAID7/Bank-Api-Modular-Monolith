using Bank.Deposits.Application.Interfaces;
using Bank.Deposits.Application.Queries;

namespace Bank.Deposits.Application.Handlers;

/// <summary>
/// DTO for maturity calculation result
/// </summary>
public class MaturityAmountDto
{
    public decimal Principal { get; set; }
    public decimal Interest { get; set; }
    public decimal MaturityAmount { get; set; }
    public decimal AnnualRate { get; set; }
    public int TermMonths { get; set; }
}

/// <summary>
/// Handler for CalculateMaturityAmountQuery
/// </summary>
public class CalculateMaturityAmountQueryHandler : IQueryHandler<CalculateMaturityAmountQuery, MaturityAmountDto>
{
    private readonly IInterestCalculationService _interestCalculationService;

    public CalculateMaturityAmountQueryHandler(IInterestCalculationService interestCalculationService)
    {
        _interestCalculationService = interestCalculationService;
    }

    public Task<MaturityAmountDto> Handle(CalculateMaturityAmountQuery query, CancellationToken cancellationToken)
    {
        var maturityAmount = _interestCalculationService.CalculateMaturityAmount(
            query.Principal,
            query.AnnualRate,
            query.TermMonths,
            query.CalculationMethod);

        var interest = maturityAmount - query.Principal;

        var result = new MaturityAmountDto
        {
            Principal = query.Principal,
            Interest = interest,
            MaturityAmount = maturityAmount,
            AnnualRate = query.AnnualRate,
            TermMonths = query.TermMonths
        };

        return Task.FromResult(result);
    }
}
