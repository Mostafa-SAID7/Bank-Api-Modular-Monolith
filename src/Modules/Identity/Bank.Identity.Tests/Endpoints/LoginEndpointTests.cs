namespace Bank.Identity.Tests.Endpoints;

/// <summary>
/// Integration tests for user login endpoint
/// POST /api/v1/identity/login
/// </summary>
[Collection("Identity Integration")]
public class LoginEndpointTests : IAsyncLifetime
{
    private readonly IdentityTestFixture _fixture;
    private HttpClient _client = null!;

    public LoginEndpointTests()
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
    public async Task Login_WithValidCredentials_ReturnsOkWithTokens()
    {
        // Arrange - Register a user first
        var registerRequest = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        await _client.PostAsJsonAsync("/api/v1/identity/register", registerRequest);

        var loginRequest = new LoginRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!"
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/login",
            loginRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsAsync<LoginResponse>();
        content.Should().NotBeNull();
        content!.Email.Should().Be("john.doe@example.com");
        content.AccessToken.Should().NotBeNullOrEmpty();
        content.RefreshToken.Should().NotBeNullOrEmpty();
        content.SessionId.Should().NotBe(Guid.Empty);
        content.Success.Should().BeTrue();
        content.ExpiresAtUtc.Should().BeGreaterThan(DateTime.UtcNow);
    }

    [Fact]
    public async Task Login_WithIncorrectPassword_ReturnsUnauthorized()
    {
        // Arrange - Register a user first
        var registerRequest = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        await _client.PostAsJsonAsync("/api/v1/identity/register", registerRequest);

        var loginRequest = new LoginRequest(
            Email: "john.doe@example.com",
            Password: "WrongPassword123!"
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/login",
            loginRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithNonExistentUser_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest(
            Email: "nonexistent@example.com",
            Password: "SecurePass123!"
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/login",
            loginRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithEmptyEmail_ReturnsBadRequest()
    {
        // Arrange
        var loginRequest = new LoginRequest(
            Email: "",
            Password: "SecurePass123!"
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/login",
            loginRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsAsync<ErrorResponse>();
        content.Should().NotBeNull();
    }

    [Fact]
    public async Task Login_WithEmptyPassword_ReturnsBadRequest()
    {
        // Arrange
        var loginRequest = new LoginRequest(
            Email: "john.doe@example.com",
            Password: ""
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/login",
            loginRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsAsync<ErrorResponse>();
        content.Should().NotBeNull();
    }

    [Fact]
    public async Task Login_WithInvalidEmailFormat_ReturnsBadRequest()
    {
        // Arrange
        var loginRequest = new LoginRequest(
            Email: "not-an-email",
            Password: "SecurePass123!"
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/login",
            loginRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_MultipleTimesWithSameCredentials_ReturnsOkEachTime()
    {
        // Arrange - Register a user first
        var registerRequest = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        await _client.PostAsJsonAsync("/api/v1/identity/register", registerRequest);

        var loginRequest = new LoginRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!"
        );

        // Act & Assert - First login
        var response1 = await _client.PostAsJsonAsync(
            "/api/v1/identity/login",
            loginRequest
        );
        response1.StatusCode.Should().Be(HttpStatusCode.OK);

        var content1 = await response1.Content.ReadAsAsync<LoginResponse>();
        content1!.SessionId.Should().NotBe(Guid.Empty);

        // Act & Assert - Second login
        var response2 = await _client.PostAsJsonAsync(
            "/api/v1/identity/login",
            loginRequest
        );
        response2.StatusCode.Should().Be(HttpStatusCode.OK);

        var content2 = await response2.Content.ReadAsAsync<LoginResponse>();
        content2!.SessionId.Should().NotBe(Guid.Empty);

        // Sessions should be different
        content1.SessionId.Should().NotBe(content2.SessionId);
    }

    [Fact]
    public async Task Login_CreatesSessionInDatabase()
    {
        // Arrange - Register a user first
        var registerRequest = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        await _client.PostAsJsonAsync("/api/v1/identity/register", registerRequest);

        var loginRequest = new LoginRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!"
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/login",
            loginRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsAsync<LoginResponse>();
        var sessionId = content!.SessionId;

        // Verify session exists in database
        var session = await _fixture.GetSessionAsync(sessionId);
        session.Should().NotBeNull();
        session!.UserId.Should().Be(content.UserId);
    }

    [Fact]
    public async Task Login_UpdatesLastLoginAtUtc()
    {
        // Arrange - Register a user first
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
        var lastLoginBefore = user1!.LastLoginAtUtc;

        var loginRequest = new LoginRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!"
        );

        // Wait a bit to ensure time difference
        await Task.Delay(100);

        // Act
        await _client.PostAsJsonAsync("/api/v1/identity/login", loginRequest);

        // Assert
        var user2 = await _fixture.GetUserAsync(userId);
        user2!.LastLoginAtUtc.Should().BeGreaterThan(lastLoginBefore ?? DateTime.MinValue);
    }

    [Fact]
    public async Task Login_ReturnsAccessTokenWithValidStructure()
    {
        // Arrange - Register a user first
        var registerRequest = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        await _client.PostAsJsonAsync("/api/v1/identity/register", registerRequest);

        var loginRequest = new LoginRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!"
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/login",
            loginRequest
        );

        // Assert
        var content = await response.Content.ReadAsAsync<LoginResponse>();
        content!.AccessToken.Should().NotBeNullOrEmpty();

        // JWT tokens have 3 parts separated by dots
        var tokenParts = content.AccessToken.Split('.');
        tokenParts.Should().HaveLength(3);
    }
}
