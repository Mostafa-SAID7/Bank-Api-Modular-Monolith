namespace Bank.Loans.Application.Interfaces;

/// <summary>
/// Service for generating unique loan numbers
/// </summary>
public interface ILoanNumberGenerator
{
    /// <summary>
    /// Generate a unique loan number
    /// Format: LN + YYYYMMDD + 6-digit sequence
    /// Example: LN202609201234567
    /// </summary>
    Task<string> GenerateAsync(CancellationToken cancellationToken = default);
}
