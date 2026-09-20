namespace Bank.Identity.Application.Commands;

/// <summary>
/// Command to refresh an access token using refresh token
/// </summary>
public sealed record RefreshTokenCommand(
    string RefreshToken,
    string IpAddress,
    string UserAgent)
{
    public RefreshTokenCommand() : this("", "", "") { }
}

/// <summary>
/// Result of token refresh command
/// </summary>
public sealed record RefreshTokenResult(
    Guid UserId,
    string AccessToken,
    string NewRefreshToken,
    DateTime ExpiresAtUtc,
    bool Success,
    string? Message = null);
