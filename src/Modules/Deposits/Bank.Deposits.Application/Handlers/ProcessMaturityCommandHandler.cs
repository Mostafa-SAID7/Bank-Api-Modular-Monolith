namespace Bank.Deposits.Application.Handlers;

/// <summary>
/// Handler for ProcessMaturityCommand
/// </summary>
public class ProcessMaturityCommandHandler : IRequestHandler<ProcessMaturityCommand, FixedDeposit>
{
    private readonly IFixedDepositRepository _fixedDepositRepository;
    private readonly IDepositProductRepository _depositProductRepository;

    public ProcessMaturityCommandHandler(
        IFixedDepositRepository fixedDepositRepository,
        IDepositProductRepository depositProductRepository)
    {
        _fixedDepositRepository = fixedDepositRepository;
        _depositProductRepository = depositProductRepository;
    }

    public async Task<FixedDeposit> Handle(ProcessMaturityCommand request, CancellationToken cancellationToken)
    {
        var fixedDeposit = await _fixedDepositRepository.GetByIdAsync(request.FixedDepositId, cancellationToken);
        if (fixedDeposit == null)
            throw new InvalidOperationException("Fixed deposit not found");

        if (!fixedDeposit.HasMatured())
            throw new InvalidOperationException("Fixed deposit has not reached maturity date yet");

        var maturityAction = (MaturityAction)request.MaturityAction;

        switch (maturityAction)
        {
            case MaturityAction.Payout:
                fixedDeposit.Close("Maturity - Payout");
                break;

            case MaturityAction.Renewal:
                if (request.RenewalProductId == null || request.RenewalTermMonths == null)
                    throw new InvalidOperationException("Renewal product and term are required for renewal");

                var renewalProduct = await _depositProductRepository.GetByIdAsync(request.RenewalProductId.Value, cancellationToken);
                if (renewalProduct == null || !renewalProduct.IsActive)
                    throw new InvalidOperationException("Renewal deposit product not found or inactive");

                if (!renewalProduct.IsTermValid(request.RenewalTermMonths.Value * 30))
                    throw new InvalidOperationException(
                        $"Renewal term outside product limits ({renewalProduct.MinimumTermDays / 30}-{renewalProduct.MaximumTermDays / 30} months)");

                var renewalTermDays = request.RenewalTermMonths.Value * 30;
                var renewedDeposit = fixedDeposit.Renew((int)renewalTermDays);
                await _fixedDepositRepository.AddAsync(renewedDeposit, cancellationToken);
                break;

            case MaturityAction.Reinvestment:
                fixedDeposit.MarkAsMatured();
                break;

            default:
                throw new InvalidOperationException($"Unknown maturity action: {maturityAction}");
        }

        await _fixedDepositRepository.UpdateAsync(fixedDeposit, cancellationToken);
        return fixedDeposit;
    }
}
