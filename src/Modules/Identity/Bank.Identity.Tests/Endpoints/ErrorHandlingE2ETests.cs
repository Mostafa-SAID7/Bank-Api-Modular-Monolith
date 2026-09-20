namespace Bank.Identity.Tests.Endpoints;

/// <summary>
/// E2E tests for error handling and edge cases
/// Covers malformed requests, boundary conditions, and error scenarios
/// </summary>
[Collection("Identity Integration")]
public class ErrorHandlingE2ETests : IAsyncLifetime
{
    private readonly IdentityTestFixture _fixture;
    private HttpClient _client = null!;

    public ErrorHandlingE2ETests()
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
    public async Task MalformedJsonRequest_ReturnsBadRequest()
    {
        // Arrange
        var malformedJson = "{ invalid json }";
        var content = new StringContent(malformedJson, System.Text.Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/identity/register", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task MissingRequiredFields_ReturnsBadRequest()
    {
        // Arrange
        var incompleteRequest = new { email = "john@example.com" }; // Missing password, firstName, lastName

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            incompleteRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ExtraneousFields_AreIgnored()
    {
        // Arrange
        var requestWithExtra = new
        {
            email = "john.doe@example.com",
            password = "SecurePass123!",
            firstName = "John",
            lastName = "Doe",
            isAdmin = true, // Extra field
            role = "Admin"   // Extra field
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            requestWithExtra
        );

        // Assert - Should succeed, extra fields ignored
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task NullValuesForRequiredFields_ReturnsBadRequest()
    {
        // Arrange
        var nullFieldJson = """
        {
            "email": null,
            "password": "SecurePass123!",
            "firstName": "John",
            "lastName": "Doe"
        }
        """;

        var content = new StringContent(nullFieldJson, System.Text.Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/identity/register", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ExcessivelyLongEmail_HandledGracefully()
    {
        // Arrange
        var longEmail = new string('a', 300) + "@example.com";
        var registerRequest = new RegisterUserRequest(
            Email: longEmail,
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
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Created);
    }

    [Fact]
    public async Task ExcessivelyLongPassword_HandledGracefully()
    {
        // Arrange
        var longPassword = new string('P', 1000) + "1!";
        var registerRequest = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: longPassword,
            FirstName: "John",
            LastName: "Doe"
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            registerRequest
        );

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Created);
    }

    [Fact]
    public async Task SpecialCharactersInNames_AllowedAndPreserved()
    {
        // Arrange
        var registerRequest = new RegisterUserRequest(
            Email: "user@example.com",
            Password: "SecurePass123!",
            FirstName: "José",
            LastName: "O'Brien"
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            registerRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var registerData = await response.Content.ReadAsAsync<RegisterUserResponse>();
        var userId = registerData!.UserId;

        var user = await _fixture.GetUserAsync(userId);
        user!.FirstName.Should().Be("José");
        user.LastName.Should().Be("O'Brien");
    }

    [Fact]
    public async Task InvalidGuidFormat_ReturnsBadRequest()
    {
        // Arrange
        var invalidGuid = "not-a-guid";

        // Act
        var response = await _client.GetAsync($"/api/v1/identity/users/{invalidGuid}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task NonExistentEndpoint_ReturnsNotFound()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/api/v1/identity/nonexistent");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WrongHttpMethod_ReturnsMethodNotAllowed()
    {
        // Arrange - GET instead of POST for login
        var loginRequest = new LoginRequest(
            Email: "john@example.com",
            Password: "SecurePass123!"
        );

        // Act
        var response = await _client.GetAsync("/api/v1/identity/login");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
    }

    [Fact]
    public async Task EmptyRequestBody_ReturnsBadRequest()
    {
        // Arrange
        var emptyContent = new StringContent("", System.Text.Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/identity/register", emptyContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WhitespaceOnlyFields_ReturnsBadRequest()
    {
        // Arrange
        var registerRequest = new RegisterUserRequest(
            Email: "   ",
            Password: "   ",
            FirstName: "   ",
            LastName: "   "
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
    public async Task IncorrectContentType_MayBeHandled()
    {
        // Arrange
        var content = new StringContent(
            "email=john@example.com&password=SecurePass123!&firstName=John&lastName=Doe",
            System.Text.Encoding.UTF8,
            "application/x-www-form-urlencoded"
        );

        // Act
        var response = await _client.PostAsync("/api/v1/identity/register", content);

        // Assert - Should reject or attempt to parse based on implementation
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.UnsupportedMediaType);
    }

    [Fact]
    public async Task CaseInsensitiveEmailCheck_DuplicateDetection()
    {
        // Arrange - Register with one case
        var registerRequest1 = new RegisterUserRequest(
            Email: "John.Doe@Example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        await _client.PostAsJsonAsync("/api/v1/identity/register", registerRequest1);

        // Try to register with different case
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

        // Assert - Should detect as duplicate
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task LoginWithNonExistentUser_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest(
            Email: "nonexistent@example.com",
            Password: "AnyPassword123!"
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
    public async Task LogoutWithInvalidSessionId_ReturnsBadRequest()
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
    }

    [Fact]
    public async Task GetUserWithDeletedUser_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/identity/users/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ErrorResponse_ContainsMessage()
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

        var errorResponse = await response.Content.ReadAsAsync<ErrorResponse>();
        errorResponse.Should().NotBeNull();
        errorResponse!.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task StatusCode400_ProvidedForValidationErrors()
    {
        // Arrange
        var registerRequest = new RegisterUserRequest(
            Email: "invalid-email",
            Password: "weak",
            FirstName: "",
            LastName: ""
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
    public async Task StatusCode404_ProvidedForNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/identity/users/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var errorResponse = await response.Content.ReadAsAsync<ErrorResponse>();
        errorResponse!.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task StatusCode409_ProvidedForConflict()
    {
        // Arrange
        var registerRequest = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        await _client.PostAsJsonAsync("/api/v1/identity/register", registerRequest);

        // Act - Try to register same email again
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            registerRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
