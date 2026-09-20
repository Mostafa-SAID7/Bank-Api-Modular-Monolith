namespace Bank.Identity.Domain.Events;

/// <summary>
/// Domain event: User logged in successfully
/// </summary>
public sealed record UserLoggedIn(
    Guid UserId,
    string Email,
    string IpAddress,
    DateTime LoggedInAtUtc);
