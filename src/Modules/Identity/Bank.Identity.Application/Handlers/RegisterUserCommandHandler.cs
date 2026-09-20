using Bank.Identity.Application.Commands;
using Microsoft.AspNetCore.Identity;

namespace Bank.Identity.Application.Handlers;

/// <summary>
/// Handler for RegisterUserCommand
/// Creates new user account with validation
/// </summary>
public sealed class RegisterUserCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly UserManager<User> _userManager;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        UserManager<User> userManager)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _userManager = userManager;
    }

    public async Task<RegisterUserResult> Handle(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate email format (basic check)
            if (string.IsNullOrWhiteSpace(command.Email) || !command.Email.Contains("@"))
                return new RegisterUserResult(Guid.Empty, command.Email, "", false, "Invalid email format");

            // Validate password strength (basic check)
            if (string.IsNullOrWhiteSpace(command.Password) || command.Password.Length < 8)
                return new RegisterUserResult(Guid.Empty, command.Email, "", false, "Password must be at least 8 characters");

            // Check if user already exists
            var existingUser = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);
            if (existingUser != null)
                return new RegisterUserResult(Guid.Empty, command.Email, "", false, "User with this email already exists");

            // Create user aggregate
            var user = User.Create(
                command.Email,
                command.FirstName,
                command.LastName,
                command.PhoneNumber
            );

            // Hash password and set
            var passwordHash = _passwordHasher.HashPassword(command.Password);
            user.PasswordHash = passwordHash;

            // Save to repository
            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            return new RegisterUserResult(
                user.Id,
                user.Email ?? "",
                user.FullName,
                true,
                "User registered successfully"
            );
        }
        catch (Exception ex)
        {
            return new RegisterUserResult(
                Guid.Empty,
                command.Email,
                "",
                false,
                $"Registration failed: {ex.Message}"
            );
        }
    }
}
