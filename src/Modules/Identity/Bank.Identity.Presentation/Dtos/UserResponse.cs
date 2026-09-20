namespace Bank.Identity.Presentation.Dtos;

/// <summary>
/// Response DTO for user details
/// </summary>
public sealed record UserResponse(
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
