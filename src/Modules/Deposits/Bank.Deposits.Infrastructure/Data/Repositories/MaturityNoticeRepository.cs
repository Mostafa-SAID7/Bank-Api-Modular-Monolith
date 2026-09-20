namespace Bank.Deposits.Infrastructure.Data.Repositories;

/// <summary>
/// Repository for MaturityNotice persistence operations
/// </summary>
public sealed class MaturityNoticeRepository : IMaturityNoticeRepository
{
    private readonly DepositsDbContext _context;

    public MaturityNoticeRepository(DepositsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(MaturityNotice maturityNotice, CancellationToken cancellationToken = default)
    {
        await _context.MaturityNotices.AddAsync(maturityNotice, cancellationToken);
    }

    public async Task<MaturityNotice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MaturityNotices
            .FirstOrDefaultAsync(mn => mn.Id == id, cancellationToken);
    }

    public async Task<MaturityNotice?> GetByFixedDepositIdAsync(Guid fixedDepositId, CancellationToken cancellationToken = default)
    {
        return await _context.MaturityNotices
            .FirstOrDefaultAsync(mn => mn.FixedDepositId == fixedDepositId, cancellationToken);
    }

    public async Task<List<MaturityNotice>> GetPendingNoticesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.MaturityNotices
            .Where(mn => !mn.NotificationSent)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MaturityNotice>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.MaturityNotices
            .Where(mn => mn.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(MaturityNotice maturityNotice, CancellationToken cancellationToken = default)
    {
        _context.MaturityNotices.Update(maturityNotice);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var maturityNotice = await GetByIdAsync(id, cancellationToken);
        if (maturityNotice is not null)
        {
            _context.MaturityNotices.Remove(maturityNotice);
        }
    }
}
