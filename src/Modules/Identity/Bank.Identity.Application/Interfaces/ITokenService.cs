namespace Bank.Identity.Application.Interfaces;

/// <summary>
/// Service for generating and validating JWT tokens
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generate access token for user
    /// </summary>
    string GenerateAccessToken(User user, IEnumerable<string> roles);

    /// <summary>
    /// Generate refresh token
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Validate and extract claims from access token
    /// </summary>
    /// <returns>User ID if valid, null otherwise</returns>
    Guid? ValidateAccessToken(string token);

    /// <summary>
    /// Get access token expiration time
    /// </summary>
    TimeSpan GetAccessTokenExpiration();

    /// <summary>
    /// Get refresh token expiration time
    /// </summary>
    TimeSpan GetRefreshTokenExpiration();
}
