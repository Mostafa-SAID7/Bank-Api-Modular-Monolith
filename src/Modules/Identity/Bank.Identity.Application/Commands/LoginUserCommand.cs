namespace Bank.Identity.Application.Commands;

/// <summary>
/// Command to authenticate and log in a user
/// </summary>
public sealed record LoginUserCommand(
    string Email,
    string Password,
    string IpAddress,
    string UserAgent,
    string? DeviceFingerprint = null)
{
    public LoginUserCommand() : this("", "", "", "") { }
}

/// <summary>
/// Result of login command
/// </summary>
public sealed record LoginUserResult(
    Guid UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    Guid SessionId,
    DateTime ExpiresAtUtc,
    bool Success,
    string? Message = null);
