namespace Bank.Identity.Application.Commands;

/// <summary>
/// Command to log out a user and terminate their session
/// </summary>
public sealed record LogoutUserCommand(
    Guid SessionId,
    Guid UserId,
    string Reason = "User initiated logout")
{
    public LogoutUserCommand() : this(Guid.Empty, Guid.Empty) { }
}

/// <summary>
/// Result of logout command
/// </summary>
public sealed record LogoutUserResult(
    Guid SessionId,
    bool Success,
    string? Message = null);
