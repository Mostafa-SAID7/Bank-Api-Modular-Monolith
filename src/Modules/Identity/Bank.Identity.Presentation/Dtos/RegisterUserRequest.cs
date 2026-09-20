namespace Bank.Identity.Presentation.Dtos;

/// <summary>
/// Request DTO for user registration
/// </summary>
public sealed record RegisterUserRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? PhoneNumber = null)
{
    public RegisterUserRequest() : this("", "", "", "") { }
}

/// <summary>
/// Response DTO for user registration
/// </summary>
public sealed record RegisterUserResponse(
    Guid UserId,
    string Email,
    string FullName,
    bool Success,
    string? Message = null);
