using Bank.Identity.Application.Commands;

namespace Bank.Identity.Application.Handlers;

/// <summary>
/// Handler for LoginUserCommand
/// Authenticates user and creates session
/// </summary>
public sealed class LoginUserCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        ISessionRepository sessionRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LoginUserResult> Handle(
        LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            // Find user by email
            var user = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);
            if (user == null)
                return new LoginUserResult(
                    Guid.Empty, command.Email, "", "", Guid.Empty, DateTime.UtcNow, false,
                    "Invalid email or password"
                );

            // Check if user is active
            if (!user.IsActive())
                return new LoginUserResult(
                    Guid.Empty, command.Email, "", "", Guid.Empty, DateTime.UtcNow, false,
                    "User account is not active"
                );

            // Verify password
            if (user.PasswordHash == null || !_passwordHasher.VerifyPassword(command.Password, user.PasswordHash))
            {
                user.RecordFailedLogin();
                await _userRepository.UpdateAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);

                return new LoginUserResult(
                    Guid.Empty, command.Email, "", "", Guid.Empty, DateTime.UtcNow, false,
                    "Invalid email or password"
                );
            }

            // Generate tokens
            var accessToken = _tokenService.GenerateAccessToken(user, new[] { "user" });
            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenHash = _passwordHasher.HashPassword(refreshToken); // Hash refresh token

            // Create session
            var session = Session.Create(
                user.Id,
                accessToken,
                refreshTokenHash,
                _tokenService.GetAccessTokenExpiration(),
                _tokenService.GetRefreshTokenExpiration(),
                command.IpAddress,
                command.UserAgent,
                command.DeviceFingerprint
            );

            await _sessionRepository.AddAsync(session, cancellationToken);

            // Record successful login
            user.RecordLogin();
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            return new LoginUserResult(
                user.Id,
                user.Email ?? "",
                accessToken,
                refreshToken,
                session.Id,
                session.ExpiresAtUtc,
                true,
                "Login successful"
            );
        }
        catch (Exception ex)
        {
            return new LoginUserResult(
                Guid.Empty, command.Email, "", "", Guid.Empty, DateTime.UtcNow, false,
                $"Login failed: {ex.Message}"
            );
        }
    }
}
