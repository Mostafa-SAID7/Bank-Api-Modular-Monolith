namespace Bank.Deposits.Application.Handlers;

/// <summary>
/// Handler for GetFixedDepositQuery
/// </summary>
public class GetFixedDepositQueryHandler : IRequestHandler<GetFixedDepositQuery, FixedDepositDto>
{
    private readonly IFixedDepositRepository _fixedDepositRepository;

    public GetFixedDepositQueryHandler(IFixedDepositRepository fixedDepositRepository)
    {
        _fixedDepositRepository = fixedDepositRepository;
    }

    public async Task<FixedDepositDto> Handle(GetFixedDepositQuery request, CancellationToken cancellationToken)
    {
        var fixedDeposit = await _fixedDepositRepository.GetByIdAsync(request.FixedDepositId, cancellationToken);
        if (fixedDeposit == null)
            throw new InvalidOperationException("Fixed deposit not found");

        return MapToDto(fixedDeposit);
    }

    private static FixedDepositDto MapToDto(FixedDeposit fixedDeposit)
    {
        return new FixedDepositDto(
            fixedDeposit.Id,
            fixedDeposit.DepositNumber,
            fixedDeposit.CustomerId,
            fixedDeposit.DepositProductId,
            fixedDeposit.PrincipalAmount,
            fixedDeposit.Status,
            fixedDeposit.InterestRate,
            fixedDeposit.TermDays / 30, // Convert days to approximate months
            fixedDeposit.StartDateUtc,
            fixedDeposit.MaturityDateUtc,
            fixedDeposit.AccruedInterest,
            fixedDeposit.CompoundingFrequency,
            fixedDeposit.InterestCalculationMethod,
            fixedDeposit.AutoRenewalEnabled,
            fixedDeposit.PenaltyPercentage ?? 0,
            fixedDeposit.CreatedAtUtc,
            fixedDeposit.UpdatedAtUtc);
    }
}
