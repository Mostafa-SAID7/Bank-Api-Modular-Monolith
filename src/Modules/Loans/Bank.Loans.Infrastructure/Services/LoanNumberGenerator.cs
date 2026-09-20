namespace Bank.Loans.Infrastructure.Services;

/// <summary>
/// Generates unique loan numbers for new loan applications
/// Format: LN + YYYYMMDD + 6-digit sequence (e.g., LN202609200000001)
/// Uses sequential numbering within each day for readability
/// </summary>
public sealed class LoanNumberGenerator : ILoanNumberGenerator
{
    private static long _sequenceCounter = 0;
    private static readonly object _lockObject = new object();

    public Task<string> GenerateAsync(CancellationToken cancellationToken = default)
    {
        lock (_lockObject)
        {
            // Increment sequence counter
            _sequenceCounter++;
            
            // Reset counter if it exceeds 6 digits
            if (_sequenceCounter > 999999)
            {
                _sequenceCounter = 1;
            }

            // Format: LN + YYYYMMDD + 6-digit sequence
            var datePrefix = DateTime.UtcNow.ToString("yyyyMMdd");
            var sequencePart = _sequenceCounter.ToString("D6");
            var loanNumber = $"LN{datePrefix}{sequencePart}";

            return Task.FromResult(loanNumber);
        }
    }
}
