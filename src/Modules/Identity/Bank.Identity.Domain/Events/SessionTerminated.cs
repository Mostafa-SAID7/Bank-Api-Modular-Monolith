namespace Bank.Identity.Domain.Events;

/// <summary>
/// Domain event: Session was terminated
/// </summary>
public sealed record SessionTerminated(
    Guid SessionId,
    Guid UserId,
    string Reason,
    DateTime TerminatedAtUtc);
