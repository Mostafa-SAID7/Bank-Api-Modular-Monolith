using Bank.Identity.Domain.Enums;

namespace Bank.Identity.Domain.Entities;

/// <summary>
/// Session aggregate root - tracks active user sessions
/// Supports refresh token rotation and concurrent session management
/// Migrated from Bank.Domain to Identity module
/// </summary>
public class Session
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string SessionToken { get; set; } = string.Empty;
    public string? RefreshTokenHash { get; set; }
    public string? ReplacedByToken { get; set; }
    
    // Timing
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? RefreshTokenExpiresAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime LastActivityAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? TerminatedAtUtc { get; set; }
    
    // Status & metadata
    public SessionStatus Status { get; set; } = SessionStatus.Active;
    public string? TerminationReason { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string? DeviceFingerprint { get; set; }
    public bool IsAdminSession { get; set; } = false;
    
    // Navigation
    public User? User { get; set; }

    /// <summary>
    /// Factory method for creating a new session
    /// </summary>
    public static Session Create(
        Guid userId,
        string sessionToken,
        string? refreshTokenHash,
        TimeSpan sessionTimeout,
        TimeSpan? refreshTokenTimeout,
        string ipAddress,
        string userAgent,
        string? deviceFingerprint = null,
        bool isAdminSession = false)
    {
        var now = DateTime.UtcNow;
        return new Session
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            SessionToken = sessionToken,
            RefreshTokenHash = refreshTokenHash,
            ExpiresAtUtc = now.Add(sessionTimeout),
            RefreshTokenExpiresAtUtc = refreshTokenTimeout.HasValue ? now.Add(refreshTokenTimeout.Value) : null,
            Status = SessionStatus.Active,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            DeviceFingerprint = deviceFingerprint,
            IsAdminSession = isAdminSession,
            CreatedAtUtc = now,
            LastActivityAtUtc = now
        };
    }

    /// <summary>
    /// Check if session is expired
    /// </summary>
    public bool IsExpired()
    {
        return DateTime.UtcNow > ExpiresAtUtc;
    }

    /// <summary>
    /// Check if refresh token is expired
    /// </summary>
    public bool IsRefreshTokenExpired()
    {
        return RefreshTokenExpiresAtUtc.HasValue && DateTime.UtcNow > RefreshTokenExpiresAtUtc.Value;
    }

    /// <summary>
    /// Check if session is valid (active and not expired)
    /// </summary>
    public bool IsValid()
    {
        return Status == SessionStatus.Active && !IsExpired();
    }

    /// <summary>
    /// Check if session is revoked
    /// </summary>
    public bool IsRevoked()
    {
        return TerminatedAtUtc.HasValue || Status == SessionStatus.Terminated || Status == SessionStatus.Locked;
    }

    /// <summary>
    /// Update last activity timestamp
    /// </summary>
    public void UpdateActivity()
    {
        if (Status == SessionStatus.Active)
        {
            LastActivityAtUtc = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Extend session expiration
    /// </summary>
    public void ExtendSession(TimeSpan additionalTime)
    {
        if (Status == SessionStatus.Active && !IsExpired())
        {
            ExpiresAtUtc = DateTime.UtcNow.Add(additionalTime);
            LastActivityAtUtc = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Terminate the session
    /// </summary>
    public void Terminate(string reason)
    {
        Status = SessionStatus.Terminated;
        TerminatedAtUtc = DateTime.UtcNow;
        TerminationReason = reason;
    }

    /// <summary>
    /// Lock the session (security issue)
    /// </summary>
    public void Lock(string reason)
    {
        Status = SessionStatus.Locked;
        TerminatedAtUtc = DateTime.UtcNow;
        TerminationReason = reason;
    }

    /// <summary>
    /// Replace with new session (token rotation)
    /// </summary>
    public void ReplaceWithNew(string newSessionToken)
    {
        Status = SessionStatus.Terminated;
        TerminatedAtUtc = DateTime.UtcNow;
        TerminationReason = "Token Rotated";
        ReplacedByToken = newSessionToken;
    }
}
