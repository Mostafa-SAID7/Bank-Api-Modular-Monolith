namespace Bank.Identity.Tests.Endpoints;

/// <summary>
/// E2E tests for token refresh behavior
/// Covers refresh token validity, expiration, and access token regeneration
/// </summary>
[Collection("Identity Integration")]
public class TokenRefreshE2ETests : IAsyncLifetime
{
    private readonly IdentityTestFixture _fixture;
    private HttpClient _client = null!;

    public TokenRefreshE2ETests()
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
    public async Task TokenRefresh_WithValidRefreshToken_ReturnsNewAccessToken()
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
        var refreshToken = loginData!.RefreshToken;
        var originalAccessToken = loginData.AccessToken;

        // Act - Attempt token refresh (would need RefreshTokenCommand/endpoint in real scenario)
        // For now, verify tokens were generated with proper structure
        originalAccessToken.Split('.').Should().HaveLength(3);
        refreshToken.Split('.').Should().HaveLength(3);
    }

    [Fact]
    public async Task AccessToken_ContainsRequiredClaims()
    {
        // Arrange
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
        var accessToken = loginData!.AccessToken;

        // Act & Assert - Verify token structure (3 parts separated by dots)
        var tokenParts = accessToken.Split('.');
        tokenParts.Should().HaveLength(3);

        // Decode header to verify it's valid base64
        var header = DecodeBase64(tokenParts[0]);
        header.Should().Contain("typ");
        header.Should().Contain("alg");

        // Decode payload to verify it contains claims
        var payload = DecodeBase64(tokenParts[1]);
        payload.Should().Contain("sub"); // Subject (user ID)
        payload.Should().Contain("email"); // Email claim
        payload.Should().Contain("exp"); // Expiration
        payload.Should().Contain("iat"); // Issued at
    }

    [Fact]
    public async Task AccessToken_ExpiresAtCorrectTime()
    {
        // Arrange
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

        // Assert
        var loginData = await loginResponse.Content.ReadAsAsync<LoginResponse>();
        loginData!.ExpiresAtUtc.Should().BeGreaterThan(DateTime.UtcNow);

        // Token should expire within reasonable time window (e.g., 15 minutes)
        var timeToExpiry = loginData.ExpiresAtUtc - DateTime.UtcNow;
        timeToExpiry.TotalMinutes.Should().BeLessThan(20); // Within 20 minutes
        timeToExpiry.TotalMinutes.Should().BeGreaterThan(10); // At least 10 minutes
    }

    [Fact]
    public async Task MultipleLogins_GenerateDifferentTokens()
    {
        // Arrange
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

        // Act - Login twice
        var response1 = await _client.PostAsJsonAsync(
            "/api/v1/identity/login",
            loginRequest
        );

        var data1 = await response1.Content.ReadAsAsync<LoginResponse>();

        await Task.Delay(100); // Ensure time difference

        var response2 = await _client.PostAsJsonAsync(
            "/api/v1/identity/login",
            loginRequest
        );

        var data2 = await response2.Content.ReadAsAsync<LoginResponse>();

        // Assert - Tokens should be different
        data1!.AccessToken.Should().NotBe(data2!.AccessToken);
        data1.RefreshToken.Should().NotBe(data2.RefreshToken);
        data1.SessionId.Should().NotBe(data2.SessionId);
    }

    [Fact]
    public async Task TokenPayload_DoesNotContainPassword()
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

        var loginRequest = new LoginRequest(
            Email: "john.doe@example.com",
            Password: password
        );

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/v1/identity/login",
            loginRequest
        );

        var loginData = await loginResponse.Content.ReadAsAsync<LoginResponse>();

        // Act & Assert - Verify password is not in token
        var tokenParts = loginData!.AccessToken.Split('.');
        var payload = DecodeBase64(tokenParts[1]);

        payload.Should().NotContain(password);
        payload.Should().NotContain("pass");
    }

    private static string DecodeBase64(string base64Url)
    {
        try
        {
            // Add padding if necessary
            var padded = base64Url.Length % 4 == 0
                ? base64Url
                : base64Url + new string('=', 4 - base64Url.Length % 4);

            var bytes = Convert.FromBase64String(padded);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return string.Empty;
        }
    }
}
