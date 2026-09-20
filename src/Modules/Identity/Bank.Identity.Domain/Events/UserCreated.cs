namespace Bank.Identity.Domain.Events;

/// <summary>
/// Domain event: User account created
/// </summary>
public sealed record UserCreated(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    DateTime CreatedAtUtc);
