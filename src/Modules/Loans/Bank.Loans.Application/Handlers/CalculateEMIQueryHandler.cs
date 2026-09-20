namespace Bank.Loans.Application.Handlers;

public class CalculateEMIQueryHandler : IRequestHandler<CalculateEMIQuery, EMICalculationDto>
{
    private readonly IEMICalculationService _emiCalculationService;

    public CalculateEMIQueryHandler(IEMICalculationService emiCalculationService)
    {
        _emiCalculationService = emiCalculationService;
    }

    public Task<EMICalculationDto> Handle(CalculateEMIQuery request, CancellationToken cancellationToken)
    {
        var monthlyEMI = _emiCalculationService.CalculateMonthlyEMI(
            request.Principal,
            request.AnnualRate,
            request.TenureMonths);

        var totalAmount = monthlyEMI * request.TenureMonths;
        var totalInterest = totalAmount - request.Principal;

        var dto = new EMICalculationDto(
            request.Principal,
            request.AnnualRate,
            request.TenureMonths,
            monthlyEMI,
            totalAmount,
            totalInterest);

        return Task.FromResult(dto);
    }
}
