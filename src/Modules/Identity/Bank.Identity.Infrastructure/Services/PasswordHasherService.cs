using Bank.Identity.Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Bank.Identity.Infrastructure.Services;

/// <summary>
/// Password hasher service using ASP.NET Core Identity's PasswordHasher
/// </summary>
public sealed class PasswordHasherService : IPasswordHasher
{
    private readonly IPasswordHasher<User> _passwordHasher;

    public PasswordHasherService(IPasswordHasher<User> passwordHasher)
    {
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));

        // Create a temporary user object for hashing (only used for the hash function)
        var tempUser = new User { Id = Guid.NewGuid(), Email = "temp@temp.com" };
        return _passwordHasher.HashPassword(tempUser, password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
            return false;

        // Create a temporary user object for verification (only used for the verify function)
        var tempUser = new User { Id = Guid.NewGuid(), Email = "temp@temp.com" };
        var result = _passwordHasher.VerifyHashedPassword(tempUser, hash, password);
        return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}
