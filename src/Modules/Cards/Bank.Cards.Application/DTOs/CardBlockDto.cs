namespace Bank.Cards.Application.DTOs;

/// <summary>
/// DTO for CardBlock entity
/// </summary>
public sealed record CardBlockDto(
    Guid Id,
    Guid CardId,
    BlockReason Reason,
    string Description,
    DateTime BlockedAtUtc,
    DateTime? UnblockedAtUtc);
