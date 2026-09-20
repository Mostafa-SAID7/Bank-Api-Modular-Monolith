using Bank.Identity.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Bank.Identity.Domain.Entities;

/// <summary>
/// User aggregate root - extends ASP.NET Core Identity
/// Migrated from Bank.Domain to Identity module
/// Uses Guid as primary key
/// </summary>
public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    
    // Audit
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Soft delete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAtUtc { get; set; }
    public string? DeletedBy { get; set; }
    
    // User status
    public UserStatus Status { get; set; } = UserStatus.Active;
    public DateTime? LastLoginAtUtc { get; set; }
    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LockoutStartAtUtc { get; set; }
    
    // Navigation
    public ICollection<Session> Sessions { get; set; } = new List<Session>();

    /// <summary>
    /// Factory method for creating a new user
    /// </summary>
    public static User Create(
        string email,
        string firstName,
        string lastName,
        string? phoneNumber = null)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            Status = UserStatus.Active,
            CreatedAtUtc = DateTime.UtcNow,
            EmailConfirmed = false
        };
    }

    /// <summary>
    /// Soft delete the user
    /// </summary>
    public void SoftDelete(string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
        DeletedBy = deletedBy;
        Status = UserStatus.Deleted;
    }

    /// <summary>
    /// Restore a soft-deleted user
    /// </summary>
    public void Restore()
    {
        IsDeleted = false;
        DeletedAtUtc = null;
        DeletedBy = null;
        Status = UserStatus.Active;
    }

    /// <summary>
    /// Record a successful login
    /// </summary>
    public void RecordLogin()
    {
        LastLoginAtUtc = DateTime.UtcNow;
        FailedLoginAttempts = 0;
        LockoutStartAtUtc = null;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Record a failed login attempt
    /// </summary>
    public void RecordFailedLogin(int maxAttempts = 5)
    {
        FailedLoginAttempts++;
        if (FailedLoginAttempts >= maxAttempts)
        {
            LockoutStartAtUtc = DateTime.UtcNow;
            Status = UserStatus.Locked;
        }
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Unlock a locked user account
    /// </summary>
    public void Unlock()
    {
        Status = UserStatus.Active;
        FailedLoginAttempts = 0;
        LockoutStartAtUtc = null;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Suspend the user account
    /// </summary>
    public void Suspend(string? reason = null)
    {
        Status = UserStatus.Suspended;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Check if user account is active
    /// </summary>
    public bool IsActive()
    {
        return Status == UserStatus.Active && !IsDeleted;
    }
}
