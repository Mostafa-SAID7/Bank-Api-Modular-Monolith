namespace Bank.Statements.Application.Interfaces;

using Bank.Statements.Domain.Entities;

public interface IStatementGenerator
{
    /// <summary>
    /// Generate statement lines for a given period based on posting events
    /// </summary>
    Task<IEnumerable<StatementLine>> GenerateLinesAsync(
        Guid accountId,
        DateTime periodStartDate,
        DateTime periodEndDate,
        decimal openingBalance,
        CancellationToken ct = default);

    /// <summary>
    /// Calculate closing balance and totals for a statement
    /// </summary>
    Task<(decimal ClosingBalance, decimal TotalDebits, decimal TotalCredits)> CalculateBalancesAsync(
        IEnumerable<StatementLine> lines,
        decimal openingBalance,
        CancellationToken ct = default);
}
