namespace Bank.Identity.Tests.Endpoints;

/// <summary>
/// Integration tests for get user endpoint
/// GET /api/v1/identity/users/{id}
/// </summary>
[Collection("Identity Integration")]
public class GetUserEndpointTests : IAsyncLifetime
{
    private readonly IdentityTestFixture _fixture;
    private HttpClient _client = null!;

    public GetUserEndpointTests()
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
    public async Task GetUser_WithValidId_ReturnsOk()
    {
        // Arrange - Register a user first
        var registerRequest = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe",
            PhoneNumber: "+1234567890"
        );

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            registerRequest
        );

        var registerData = await registerResponse.Content.ReadAsAsync<RegisterUserResponse>();
        var userId = registerData!.UserId;

        // Act
        var response = await _client.GetAsync($"/api/v1/identity/users/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsAsync<UserResponse>();
        content.Should().NotBeNull();
        content!.Id.Should().Be(userId);
        content.Email.Should().Be("john.doe@example.com");
        content.FirstName.Should().Be("John");
        content.LastName.Should().Be("Doe");
        content.FullName.Should().Be("John Doe");
        content.PhoneNumber.Should().Be("+1234567890");
        content.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetUser_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/identity/users/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await response.Content.ReadAsAsync<ErrorResponse>();
        content.Should().NotBeNull();
        content!.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task GetUser_WithEmptyId_ReturnsBadRequest()
    {
        // Arrange
        var emptyId = Guid.Empty;

        // Act
        var response = await _client.GetAsync($"/api/v1/identity/users/{emptyId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsAsync<ErrorResponse>();
        content.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUser_ReturnsCorrectUserData()
    {
        // Arrange
        var registerRequest = new RegisterUserRequest(
            Email: "jane.smith@example.com",
            Password: "SecurePass456!",
            FirstName: "Jane",
            LastName: "Smith"
        );

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            registerRequest
        );

        var registerData = await registerResponse.Content.ReadAsAsync<RegisterUserResponse>();
        var userId = registerData!.UserId;

        // Act
        var response = await _client.GetAsync($"/api/v1/identity/users/{userId}");

        // Assert
        var content = await response.Content.ReadAsAsync<UserResponse>();
        content!.Email.Should().Be("jane.smith@example.com");
        content.FirstName.Should().Be("Jane");
        content.LastName.Should().Be("Smith");
        content.FullName.Should().Be("Jane Smith");
        content.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task GetUser_AfterLogin_ReturnsUpdatedLastLoginAtUtc()
    {
        // Arrange - Register a user
        var registerRequest = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            registerRequest
        );

        var registerData = await registerResponse.Content.ReadAsAsync<RegisterUserResponse>();
        var userId = registerData!.UserId;

        // Get user before login
        var responseBeforeLogin = await _client.GetAsync($"/api/v1/identity/users/{userId}");
        var userBeforeLogin = await responseBeforeLogin.Content.ReadAsAsync<UserResponse>();

        // Login
        var loginRequest = new LoginRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!"
        );

        await Task.Delay(100); // Ensure time difference
        await _client.PostAsJsonAsync("/api/v1/identity/login", loginRequest);

        // Act - Get user after login
        var responseAfterLogin = await _client.GetAsync($"/api/v1/identity/users/{userId}");

        // Assert
        var userAfterLogin = await responseAfterLogin.Content.ReadAsAsync<UserResponse>();
        userAfterLogin!.LastLoginAtUtc.Should().BeGreaterThan(userBeforeLogin!.LastLoginAtUtc ?? DateTime.MinValue);
    }

    [Fact]
    public async Task GetUser_ReturnsStatusAsInteger()
    {
        // Arrange - Register a user
        var registerRequest = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            registerRequest
        );

        var registerData = await registerResponse.Content.ReadAsAsync<RegisterUserResponse>();
        var userId = registerData!.UserId;

        // Act
        var response = await _client.GetAsync($"/api/v1/identity/users/{userId}");

        // Assert
        var content = await response.Content.ReadAsAsync<UserResponse>();
        content!.Status.Should().BeGreaterThanOrEqualTo(0);
        content.Status.Should().BeLessThan(10); // Assuming reasonable status enum range
    }

    [Fact]
    public async Task GetUser_WithInvalidIdFormat_ReturnsBadRequest()
    {
        // Arrange - Use invalid GUID format
        var invalidId = "not-a-guid";

        // Act
        var response = await _client.GetAsync($"/api/v1/identity/users/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetUser_WithoutPhoneNumber_ReturnsNullPhoneNumber()
    {
        // Arrange - Register a user without phone number
        var registerRequest = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            registerRequest
        );

        var registerData = await registerResponse.Content.ReadAsAsync<RegisterUserResponse>();
        var userId = registerData!.UserId;

        // Act
        var response = await _client.GetAsync($"/api/v1/identity/users/{userId}");

        // Assert
        var content = await response.Content.ReadAsAsync<UserResponse>();
        content!.PhoneNumber.Should().BeNullOrEmpty();
    }
}
