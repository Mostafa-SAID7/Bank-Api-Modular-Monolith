namespace Bank.Deposits.Application.Handlers;

/// <summary>
/// Handler for EnableAutoRenewalCommand
/// </summary>
public class EnableAutoRenewalCommandHandler : IRequestHandler<EnableAutoRenewalCommand, FixedDeposit>
{
    private readonly IFixedDepositRepository _fixedDepositRepository;

    public EnableAutoRenewalCommandHandler(IFixedDepositRepository fixedDepositRepository)
    {
        _fixedDepositRepository = fixedDepositRepository;
    }

    public async Task<FixedDeposit> Handle(EnableAutoRenewalCommand request, CancellationToken cancellationToken)
    {
        var fixedDeposit = await _fixedDepositRepository.GetByIdAsync(request.FixedDepositId, cancellationToken);
        if (fixedDeposit == null)
            throw new InvalidOperationException("Fixed deposit not found");

        if (request.EnableAutoRenewal)
        {
            // Use TermDays as the renewal term (same duration as original)
            fixedDeposit.EnableAutoRenewal(fixedDeposit.TermDays, requireConsent: true);
        }
        else
        {
            // Disable auto-renewal by not calling EnableAutoRenewal
            // Note: Domain model doesn't have a DisableAutoRenewal method, so we need to add one
            // For now, this is a limitation - the property is read-only
            throw new InvalidOperationException("Cannot disable auto-renewal - domain model limitation");
        }

        await _fixedDepositRepository.UpdateAsync(fixedDeposit, cancellationToken);
        return fixedDeposit;
    }
}
