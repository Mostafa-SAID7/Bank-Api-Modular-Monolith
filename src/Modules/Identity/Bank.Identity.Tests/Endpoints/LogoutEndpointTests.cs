namespace Bank.Identity.Tests.Endpoints;

/// <summary>
/// Integration tests for user logout endpoint
/// POST /api/v1/identity/logout
/// </summary>
[Collection("Identity Integration")]
public class LogoutEndpointTests : IAsyncLifetime
{
    private readonly IdentityTestFixture _fixture;
    private HttpClient _client = null!;

    public LogoutEndpointTests()
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
    public async Task Logout_WithValidSession_ReturnsOk()
    {
        // Arrange - Register and login
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

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/v1/identity/login",
            loginRequest
        );

        var loginData = await loginResponse.Content.ReadAsAsync<LoginResponse>();
        var sessionId = loginData!.SessionId;
        var userId = loginData.UserId;

        var logoutRequest = new LogoutUserRequest(sessionId, userId);

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/logout",
            logoutRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Logout_WithInvalidSessionId_ReturnsBadRequest()
    {
        // Arrange
        var logoutRequest = new LogoutUserRequest(
            SessionId: Guid.NewGuid(),
            UserId: Guid.NewGuid()
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/logout",
            logoutRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsAsync<ErrorResponse>();
        content.Should().NotBeNull();
    }

    [Fact]
    public async Task Logout_TerminatesSessionInDatabase()
    {
        // Arrange - Register and login
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

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/v1/identity/login",
            loginRequest
        );

        var loginData = await loginResponse.Content.ReadAsAsync<LoginResponse>();
        var sessionId = loginData!.SessionId;
        var userId = loginData.UserId;

        // Verify session is active before logout
        var sessionBefore = await _fixture.GetSessionAsync(sessionId);
        sessionBefore.Should().NotBeNull();

        var logoutRequest = new LogoutUserRequest(sessionId, userId);

        // Act
        await _client.PostAsJsonAsync("/api/v1/identity/logout", logoutRequest);

        // Assert - Session should be terminated
        var sessionAfter = await _fixture.GetSessionAsync(sessionId);
        sessionAfter.Should().NotBeNull();
        sessionAfter!.TerminatedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task Logout_WithEmptySessionId_ReturnsBadRequest()
    {
        // Arrange
        var logoutRequest = new LogoutUserRequest(
            SessionId: Guid.Empty,
            UserId: Guid.NewGuid()
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/logout",
            logoutRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Logout_WithEmptyUserId_ReturnsBadRequest()
    {
        // Arrange
        var logoutRequest = new LogoutUserRequest(
            SessionId: Guid.NewGuid(),
            UserId: Guid.Empty
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/logout",
            logoutRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Logout_MultipleTimesWithSameSession_ReturnsBadRequestSecondTime()
    {
        // Arrange - Register and login
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

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/v1/identity/login",
            loginRequest
        );

        var loginData = await loginResponse.Content.ReadAsAsync<LoginResponse>();
        var sessionId = loginData!.SessionId;
        var userId = loginData.UserId;

        var logoutRequest = new LogoutUserRequest(sessionId, userId);

        // Act & Assert - First logout
        var response1 = await _client.PostAsJsonAsync(
            "/api/v1/identity/logout",
            logoutRequest
        );
        response1.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act & Assert - Second logout with same session
        var response2 = await _client.PostAsJsonAsync(
            "/api/v1/identity/logout",
            logoutRequest
        );
        response2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Logout_WithMismatchedUserIdAndSession_ReturnsBadRequest()
    {
        // Arrange - Register and login
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

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/v1/identity/login",
            loginRequest
        );

        var loginData = await loginResponse.Content.ReadAsAsync<LoginResponse>();
        var sessionId = loginData!.SessionId;

        var logoutRequest = new LogoutUserRequest(
            SessionId: sessionId,
            UserId: Guid.NewGuid() // Different user ID
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/logout",
            logoutRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
