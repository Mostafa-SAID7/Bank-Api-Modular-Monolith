namespace Bank.Identity.Application.Interfaces;

/// <summary>
/// Service for hashing and verifying passwords
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hash a password
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verify password against hash
    /// </summary>
    /// <returns>True if password matches hash</returns>
    bool VerifyPassword(string password, string hash);
}
