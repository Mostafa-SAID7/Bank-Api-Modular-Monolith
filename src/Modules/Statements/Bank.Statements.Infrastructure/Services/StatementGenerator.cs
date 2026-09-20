namespace Bank.Statements.Infrastructure.Services;

using Bank.Statements.Application.Interfaces;
using Bank.Statements.Domain.Entities;
using Bank.Statements.Domain.Enums;

public sealed class StatementGenerator : IStatementGenerator
{
    public Task<IEnumerable<StatementLine>> GenerateLinesAsync(
        Guid accountId,
        DateTime periodStartDate,
        DateTime periodEndDate,
        decimal openingBalance,
        CancellationToken ct = default)
    {
        // Placeholder implementation - in real system, would query transaction history
        var lines = new List<StatementLine>
        {
            StatementLine.Create(
                Guid.NewGuid(),
                periodStartDate,
                periodStartDate,
                "Opening Balance",
                StatementLineType.Interest,
                0,
                openingBalance),
        };

        return Task.FromResult((IEnumerable<StatementLine>)lines);
    }

    public Task<(decimal ClosingBalance, decimal TotalDebits, decimal TotalCredits)> CalculateBalancesAsync(
        IEnumerable<StatementLine> lines,
        decimal openingBalance,
        CancellationToken ct = default)
    {
        var lineList = lines.ToList();
        var totalDebits = lineList
            .Where(l => l.LineType == StatementLineType.Debit)
            .Sum(l => l.Amount);

        var totalCredits = lineList
            .Where(l => l.LineType is StatementLineType.Credit or StatementLineType.Interest)
            .Sum(l => l.Amount);

        var closingBalance = openingBalance - totalDebits + totalCredits;

        return Task.FromResult((closingBalance, totalDebits, totalCredits));
    }
}
