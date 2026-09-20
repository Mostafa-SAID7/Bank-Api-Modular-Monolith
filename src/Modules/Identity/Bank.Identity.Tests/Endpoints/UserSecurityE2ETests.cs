namespace Bank.Identity.Tests.Endpoints;

/// <summary>
/// E2E tests for user security features
/// Covers password hashing, account lockout, password policies, and security workflows
/// </summary>
[Collection("Identity Integration")]
public class UserSecurityE2ETests : IAsyncLifetime
{
    private readonly IdentityTestFixture _fixture;
    private HttpClient _client = null!;

    public UserSecurityE2ETests()
    {
        _fixture = new IdentityTestFixture();
    }

    public async Task InitializeAsync()
    {
        await _fixture.InitializeAsync();
        _client = _fixture.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await _fixture.DisposeAsync();
        _client.Dispose();
    }

    [Fact]
    public async Task PasswordStorage_DoesNotStorePlaintext()
    {
        // Arrange
        var password = "SecurePass123!";
        var registerRequest = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: password,
            FirstName: "John",
            LastName: "Doe"
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            registerRequest
        );

        var registerData = await response.Content.ReadAsAsync<RegisterUserResponse>();
        var userId = registerData!.UserId;

        // Assert
        var user = await _fixture.GetUserAsync(userId);
        user!.PasswordHash.Should().NotBe(password);
        user.PasswordHash.Should().NotContain(password);
    }

    [Fact]
    public async Task PasswordValidation_RequiresComplexity()
    {
        // Test cases for password complexity requirements
        var weakPasswords = new[]
        {
            "password",          // No digit, no special char
            "Pass123",           // No special char
            "Pass!@#$",          // No digit
            "pass123!",          // No uppercase
            "PASS123!",          // No lowercase
            "Pass12"             // Too short, no special char
        };

        foreach (var password in weakPasswords)
        {
            // Arrange
            var registerRequest = new RegisterUserRequest(
                Email: $"test-{Guid.NewGuid()}@example.com",
                Password: password,
                FirstName: "Test",
                LastName: "User"
            );

            // Act
            var response = await _client.PostAsJsonAsync(
                "/api/v1/identity/register",
                registerRequest
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest, 
                $"Password '{password}' should be rejected");
        }
    }

    [Fact]
    public async Task PasswordValidation_AcceptsStrongPasswords()
    {
        // Arrange
        var strongPasswords = new[]
        {
            "SecurePass123!",
            "MyP@ssw0rd2024",
            "Test#Pass99",
            "Valid$Password1"
        };

        foreach (var password in strongPasswords)
        {
            var registerRequest = new RegisterUserRequest(
                Email: $"test-{Guid.NewGuid()}@example.com",
                Password: password,
                FirstName: "Test",
                LastName: "User"
            );

            // Act
            var response = await _client.PostAsJsonAsync(
                "/api/v1/identity/register",
                registerRequest
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created,
                $"Password '{password}' should be accepted");
        }
    }

    [Fact]
    public async Task LoginFailure_TrackingAttempts()
    {
        // Arrange
        var registerRequest = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/identity/register", registerRequest);
        var registerData = await registerResponse.Content.ReadAsAsync<RegisterUserResponse>();
        var userId = registerData!.UserId;

        var user1 = await _fixture.GetUserAsync(userId);
        user1!.FailedLoginAttempts.Should().Be(0);

        var loginRequest = new LoginRequest(
            Email: "john.doe@example.com",
            Password: "WrongPassword123!"
        );

        // Act - Multiple failed login attempts
        for (int i = 0; i < 3; i++)
        {
            await _client.PostAsJsonAsync("/api/v1/identity/login", loginRequest);
            await Task.Delay(50);
        }

        // Assert
        var userAfter = await _fixture.GetUserAsync(userId);
        userAfter!.FailedLoginAttempts.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task LoginSuccess_ResetsFailedAttempts()
    {
        // Arrange - Register user
        var registerRequest = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/identity/register", registerRequest);
        var registerData = await registerResponse.Content.ReadAsAsync<RegisterUserResponse>();
        var userId = registerData!.UserId;

        // Simulate failed login attempts
        var failedLoginRequest = new LoginRequest(
            Email: "john.doe@example.com",
            Password: "WrongPassword123!"
        );

        for (int i = 0; i < 2; i++)
        {
            await _client.PostAsJsonAsync("/api/v1/identity/login", failedLoginRequest);
            await Task.Delay(50);
        }

        var userAfterFailed = await _fixture.GetUserAsync(userId);
        var failedAttempts = userAfterFailed!.FailedLoginAttempts;

        // Act - Successful login
        var successLoginRequest = new LoginRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!"
        );

        await _client.PostAsJsonAsync("/api/v1/identity/login", successLoginRequest);

        // Assert
        var userAfterSuccess = await _fixture.GetUserAsync(userId);
        userAfterSuccess!.FailedLoginAttempts.Should().Be(0);
    }

    [Fact]
    public async Task AccountLockout_AfterMultipleFailedAttempts()
    {
        // Arrange - Register user
        var registerRequest = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/identity/register", registerRequest);
        var registerData = await registerResponse.Content.ReadAsAsync<RegisterUserResponse>();
        var userId = registerData!.UserId;

        var failedLoginRequest = new LoginRequest(
            Email: "john.doe@example.com",
            Password: "WrongPassword123!"
        );

        // Act - Make 5 failed login attempts (lockout threshold)
        for (int i = 0; i < 5; i++)
        {
            await _client.PostAsJsonAsync("/api/v1/identity/login", failedLoginRequest);
            await Task.Delay(50);
        }

        // Assert - User should have lockout started
        var userAfter = await _fixture.GetUserAsync(userId);
        userAfter!.FailedLoginAttempts.Should().BeGreaterThanOrEqualTo(5);
        userAfter.LockoutStartAtUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task LockedOutUser_CannotLogin()
    {
        // Arrange
        var registerRequest = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/identity/register", registerRequest);
        var registerData = await registerResponse.Content.ReadAsAsync<RegisterUserResponse>();
        var userId = registerData!.UserId;

        // Trigger lockout
        var failedLoginRequest = new LoginRequest(
            Email: "john.doe@example.com",
            Password: "WrongPassword123!"
        );

        for (int i = 0; i < 5; i++)
        {
            await _client.PostAsJsonAsync("/api/v1/identity/login", failedLoginRequest);
            await Task.Delay(50);
        }

        // Act - Try to login with correct password while locked out
        var correctLoginRequest = new LoginRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!"
        );

        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/login",
            correctLoginRequest
        );

        // Assert - Should still be unauthorized while locked out
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PasswordDifferentFromPrevious_AllowsLogin()
    {
        // Arrange
        var originalPassword = "OriginalPass123!";
        var registerRequest = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: originalPassword,
            FirstName: "John",
            LastName: "Doe"
        );

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/identity/register", registerRequest);
        var registerData = await registerResponse.Content.ReadAsAsync<RegisterUserResponse>();
        var userId = registerData!.UserId;

        // Act & Assert - Login with original password works
        var loginRequest = new LoginRequest(
            Email: "john.doe@example.com",
            Password: originalPassword
        );

        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/login",
            loginRequest
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UserStatus_ReflectsAccountState()
    {
        // Arrange
        var registerRequest = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/identity/register", registerRequest);
        var registerData = await registerResponse.Content.ReadAsAsync<RegisterUserResponse>();
        var userId = registerData!.UserId;

        // Act & Assert
        var user = await _fixture.GetUserAsync(userId);
        user.Should().NotBeNull();
        user!.Status.Should().BeGreaterThanOrEqualTo(0); // Valid status value
    }

    [Fact]
    public async Task EmailIsRequired_CannotBeNullOrEmpty()
    {
        // Arrange
        var registerRequest = new RegisterUserRequest(
            Email: "",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            registerRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task EmailIsUnique_NoDuplicateEmails()
    {
        // Arrange
        var registerRequest1 = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        await _client.PostAsJsonAsync("/api/v1/identity/register", registerRequest1);

        var registerRequest2 = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "DifferentPass456!",
            FirstName: "Jane",
            LastName: "Doe"
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            registerRequest2
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task PasswordCaseSensitivity_CorrectlyValidates()
    {
        // Arrange
        var password = "SecurePass123!";
        var registerRequest = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: password,
            FirstName: "John",
            LastName: "Doe"
        );

        await _client.PostAsJsonAsync("/api/v1/identity/register", registerRequest);

        // Try to login with different case
        var loginRequest = new LoginRequest(
            Email: "john.doe@example.com",
            Password: password.ToLower() // Different case
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/login",
            loginRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
