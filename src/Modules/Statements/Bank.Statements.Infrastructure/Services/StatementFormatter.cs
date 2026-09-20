namespace Bank.Statements.Infrastructure.Services;

using Bank.Statements.Application.Interfaces;
using Bank.Statements.Domain.Entities;
using Bank.Statements.Domain.Enums;

public sealed class StatementFormatter : IStatementFormatter
{
    public Task<byte[]> FormatAsync(
        Statement statement,
        IEnumerable<StatementLine> lines,
        StatementFormat format,
        CancellationToken ct = default)
    {
        // Placeholder implementation - in real system, would generate actual PDF/CSV/Excel/HTML
        var content = $"Statement #{statement.Id} - {format}";
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        return Task.FromResult(bytes);
    }

    public string GetFileExtension(StatementFormat format)
    {
        return format switch
        {
            StatementFormat.PDF => ".pdf",
            StatementFormat.CSV => ".csv",
            StatementFormat.Excel => ".xlsx",
            StatementFormat.HTML => ".html",
            _ => ".txt"
        };
    }

    public string GetContentType(StatementFormat format)
    {
        return format switch
        {
            StatementFormat.PDF => "application/pdf",
            StatementFormat.CSV => "text/csv",
            StatementFormat.Excel => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            StatementFormat.HTML => "text/html",
            _ => "text/plain"
        };
    }
}
