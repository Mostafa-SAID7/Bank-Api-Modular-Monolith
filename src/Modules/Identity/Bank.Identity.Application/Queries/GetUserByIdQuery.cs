namespace Bank.Identity.Application.Queries;

/// <summary>
/// Query to retrieve a user by ID
/// </summary>
public sealed record GetUserByIdQuery(Guid UserId);

/// <summary>
/// Result of get user query
/// </summary>
public sealed record GetUserByIdResult(
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
