namespace Bank.Deposits.Infrastructure.Services;

/// <summary>
/// Service for maturity processing operations
/// </summary>
public sealed class MaturityProcessingService : IMaturityProcessingService
{
    private readonly IFixedDepositRepository _fixedDepositRepository;
    private readonly IMaturityNoticeRepository _maturityNoticeRepository;

    public MaturityProcessingService(
        IFixedDepositRepository fixedDepositRepository,
        IMaturityNoticeRepository maturityNoticeRepository)
    {
        _fixedDepositRepository = fixedDepositRepository;
        _maturityNoticeRepository = maturityNoticeRepository;
    }

    public async Task<decimal> CalculateMaturityAmountAsync(FixedDeposit fixedDeposit, CancellationToken cancellationToken = default)
    {
        // Call the domain method which handles all calculation logic
        return await Task.FromResult(fixedDeposit.CalculateMaturityAmount());
    }

    public async Task<decimal> CalculateAccruedInterestAsync(FixedDeposit fixedDeposit, CancellationToken cancellationToken = default)
    {
        // Call the domain method which handles all interest calculation logic
        return await Task.FromResult(fixedDeposit.CalculateInterestAtMaturity());
    }

    public async Task ProcessMaturityForEligibleDepositsAsync(CancellationToken cancellationToken = default)
    {
        var eligibleDeposits = await _fixedDepositRepository.GetFixedDepositsReadyForMaturityAsync(cancellationToken);
        
        foreach (var deposit in eligibleDeposits)
        {
            deposit.MarkAsMatured();
            await _fixedDepositRepository.UpdateAsync(deposit, cancellationToken);

            // Send notification
            await SendMaturityNotificationAsync(deposit, cancellationToken);
        }
    }

    public async Task SendMaturityNotificationAsync(FixedDeposit fixedDeposit, CancellationToken cancellationToken = default)
    {
        // Check if notice already exists
        var existingNotice = await _maturityNoticeRepository.GetByFixedDepositIdAsync(fixedDeposit.Id, cancellationToken);
        
        if (existingNotice != null && existingNotice.NotificationSent)
            return;

        var maturityNotice = existingNotice ?? MaturityNotice.Create(
            fixedDeposit.Id,
            fixedDeposit.CustomerId,
            fixedDeposit.DepositNumber,
            fixedDeposit.MaturityDateUtc);

        maturityNotice.MarkAsSent();

        if (existingNotice != null)
            await _maturityNoticeRepository.UpdateAsync(maturityNotice, cancellationToken);
        else
            await _maturityNoticeRepository.AddAsync(maturityNotice, cancellationToken);
    }
}
