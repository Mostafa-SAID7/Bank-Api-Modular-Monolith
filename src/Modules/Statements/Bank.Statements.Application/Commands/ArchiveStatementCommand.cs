namespace Bank.Statements.Application.Commands;

using Bank.Statements.Domain.Enums;

public sealed record ArchiveStatementCommand(
    Guid StatementId,
    ArchiveReason Reason,
    string? Notes = null);
