namespace Bank.Statements.Application.Interfaces;

using Bank.Statements.Domain.Entities;
using Bank.Statements.Domain.Enums;

public interface IStatementFormatter
{
    /// <summary>
    /// Format statement into specified format (PDF, CSV, Excel, HTML)
    /// </summary>
    Task<byte[]> FormatAsync(
        Statement statement,
        IEnumerable<StatementLine> lines,
        StatementFormat format,
        CancellationToken ct = default);

    /// <summary>
    /// Get the file extension for the format
    /// </summary>
    string GetFileExtension(StatementFormat format);

    /// <summary>
    /// Get the content type for the format
    /// </summary>
    string GetContentType(StatementFormat format);
}
