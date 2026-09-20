namespace Bank.Deposits.Application.Handlers;

/// <summary>
/// Handler for CreateFixedDepositCommand
/// </summary>
public class CreateFixedDepositCommandHandler : IRequestHandler<CreateFixedDepositCommand, FixedDeposit>
{
    private readonly IFixedDepositRepository _fixedDepositRepository;
    private readonly IDepositProductRepository _depositProductRepository;
    private readonly IFixedDepositNumberGenerator _numberGenerator;

    public CreateFixedDepositCommandHandler(
        IFixedDepositRepository fixedDepositRepository,
        IDepositProductRepository depositProductRepository,
        IFixedDepositNumberGenerator numberGenerator)
    {
        _fixedDepositRepository = fixedDepositRepository;
        _depositProductRepository = depositProductRepository;
        _numberGenerator = numberGenerator;
    }

    public async Task<FixedDeposit> Handle(CreateFixedDepositCommand request, CancellationToken cancellationToken)
    {
        var depositProduct = await _depositProductRepository.GetByIdAsync(request.DepositProductId, cancellationToken);
        if (depositProduct == null || !depositProduct.IsActive)
            throw new InvalidOperationException("Deposit product not found or inactive");

        if (!depositProduct.IsAmountValid(request.PrincipalAmount))
            throw new InvalidOperationException(
                $"Principal amount outside product limits ({depositProduct.MinimumAmount}-{depositProduct.MaximumAmount})");

        if (!depositProduct.IsTermValid(request.TermMonths * 30)) // Convert months to days
            throw new InvalidOperationException(
                $"Term outside product limits ({depositProduct.MinimumTermDays / 30}-{depositProduct.MaximumTermDays / 30} months)");

        var fixedDepositNumber = await _numberGenerator.GenerateAsync(cancellationToken);
        var termDays = request.TermMonths * 30; // Convert months to days
        var maturityDate = request.StartDate.AddDays(termDays);
        
        var interestCalculationMethod = request.InterestCalculationMethod ?? depositProduct.InterestCalculationMethod;

        var fixedDeposit = FixedDeposit.Create(
            request.CustomerId,
            Guid.NewGuid(), // LinkedAccountId - would come from account service in real scenario
            request.DepositProductId,
            request.PrincipalAmount,
            termDays,
            depositProduct.BaseInterestRate,
            interestCalculationMethod,
            depositProduct.CompoundingFrequency,
            depositProduct.DefaultPenaltyType,
            depositProduct.DefaultPenaltyAmount,
            depositProduct.DefaultPenaltyPercentage);

        await _fixedDepositRepository.AddAsync(fixedDeposit, cancellationToken);
        return fixedDeposit;
    }
}
