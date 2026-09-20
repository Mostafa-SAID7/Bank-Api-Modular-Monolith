namespace Bank.Identity.Application.Queries;

/// <summary>
/// Query to retrieve a user by email
/// </summary>
public sealed record GetUserByEmailQuery(string Email);

/// <summary>
/// Result of get user by email query
/// </summary>
public sealed record GetUserByEmailResult(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    string? PhoneNumber,
    int Status,
    DateTime CreatedAtUtc,
    DateTime? LastLoginAtUtc,
    bool IsActive);
