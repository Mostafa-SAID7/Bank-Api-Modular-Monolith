namespace Bank.Deposits.Infrastructure.Services;

/// <summary>
/// Service for generating unique fixed deposit numbers
/// Format: FD-YYYYMMDD-XXXXX (e.g., FD-20260920-00001)
/// </summary>
public sealed class FixedDepositNumberGenerator : IFixedDepositNumberGenerator
{
    private readonly DepositsDbContext _context;

    public FixedDepositNumberGenerator(DepositsDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        
        // Get count of fixed deposits created today
        var countToday = await _context.FixedDeposits
            .Where(fd => fd.CreatedAtUtc.Year == DateTime.UtcNow.Year &&
                         fd.CreatedAtUtc.Month == DateTime.UtcNow.Month &&
                         fd.CreatedAtUtc.Day == DateTime.UtcNow.Day)
            .CountAsync(cancellationToken);

        var sequence = (countToday + 1).ToString("D5");
        return $"FD-{today}-{sequence}";
    }
}
