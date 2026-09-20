namespace Bank.Deposits.Application.Handlers;

/// <summary>
/// Handler for GetMaturityScheduleQuery
/// </summary>
public class GetMaturityScheduleQueryHandler : IRequestHandler<GetMaturityScheduleQuery, MaturityScheduleDto>
{
    private readonly IFixedDepositRepository _fixedDepositRepository;
    private readonly IMaturityProcessingService _maturityProcessingService;

    public GetMaturityScheduleQueryHandler(
        IFixedDepositRepository fixedDepositRepository,
        IMaturityProcessingService maturityProcessingService)
    {
        _fixedDepositRepository = fixedDepositRepository;
        _maturityProcessingService = maturityProcessingService;
    }

    public async Task<MaturityScheduleDto> Handle(GetMaturityScheduleQuery request, CancellationToken cancellationToken)
    {
        var fixedDeposit = await _fixedDepositRepository.GetByIdAsync(request.FixedDepositId, cancellationToken);
        if (fixedDeposit == null)
            throw new InvalidOperationException("Fixed deposit not found");

        var maturityAmount = await _maturityProcessingService.CalculateMaturityAmountAsync(fixedDeposit, cancellationToken);
        var remainingDays = (fixedDeposit.MaturityDateUtc - DateTime.UtcNow).Days;

        return new MaturityScheduleDto(
            fixedDeposit.Id,
            fixedDeposit.DepositNumber,
            fixedDeposit.PrincipalAmount,
            fixedDeposit.InterestRate,
            fixedDeposit.TermDays / 30, // Convert days to approximate months
            fixedDeposit.StartDateUtc,
            fixedDeposit.MaturityDateUtc,
            remainingDays,
            fixedDeposit.AccruedInterest,
            maturityAmount,
            fixedDeposit.AutoRenewalEnabled,
            fixedDeposit.Status);
    }
}
