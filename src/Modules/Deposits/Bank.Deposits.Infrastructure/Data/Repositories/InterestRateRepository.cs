namespace Bank.Deposits.Infrastructure.Data.Repositories;

/// <summary>
/// Repository for InterestRate entity persistence operations
/// Implements IInterestRateRepository interface from Application layer
/// </summary>
public sealed class InterestRateRepository : IInterestRateRepository
{
    private readonly DepositsDbContext _context;

    public InterestRateRepository(DepositsDbContext context)
    {
        _context = context;
    }

    public async Task<InterestRate?> GetCurrentRateAsync(Guid depositTypeId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await _context.InterestRates
            .Where(ir => ir.DepositTypeId == depositTypeId
                && ir.IsActive
                && ir.EffectiveFromUtc <= now
                && (ir.EffectiveToUtc == null || ir.EffectiveToUtc > now))
            .OrderByDescending(ir => ir.EffectiveFromUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<InterestRate?> GetRateForBalanceAsync(Guid depositTypeId, decimal balance, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await _context.InterestRates
            .Where(ir => ir.DepositTypeId == depositTypeId
                && ir.IsActive
                && ir.EffectiveFromUtc <= now
                && (ir.EffectiveToUtc == null || ir.EffectiveToUtc > now)
                && (ir.MinimumBalanceForRate == null || balance >= ir.MinimumBalanceForRate)
                && (ir.MaximumBalanceForRate == null || balance <= ir.MaximumBalanceForRate))
            .OrderByDescending(ir => ir.AnnualRate)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
