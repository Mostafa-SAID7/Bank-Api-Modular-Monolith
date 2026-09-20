namespace Bank.Identity.Application.Commands;

/// <summary>
/// Command to register a new user account
/// </summary>
public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? PhoneNumber = null)
{
    public RegisterUserCommand() : this("", "", "", "") { }
}

/// <summary>
/// Result of user registration command
/// </summary>
public sealed record RegisterUserResult(
    Guid UserId,
    string Email,
    string FullName,
    bool Success,
    string? Message = null);
