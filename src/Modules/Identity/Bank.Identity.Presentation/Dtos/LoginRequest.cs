namespace Bank.Identity.Presentation.Dtos;

/// <summary>
/// Request DTO for user login
/// </summary>
public sealed record LoginRequest(
    string Email,
    string Password)
{
    public LoginRequest() : this("", "") { }
}

/// <summary>
/// Response DTO for login
/// </summary>
public sealed record LoginResponse(
    Guid UserId,
    string Email,
    string FullName,
    string AccessToken,
    string RefreshToken,
    Guid SessionId,
    DateTime ExpiresAtUtc,
    bool Success,
    string? Message = null);
