namespace Bank.Identity.Tests.Endpoints;

/// <summary>
/// Integration tests for user registration endpoint
/// POST /api/v1/identity/register
/// </summary>
[Collection("Identity Integration")]
public class RegisterEndpointTests : IAsyncLifetime
{
    private readonly IdentityTestFixture _fixture;
    private HttpClient _client = null!;

    public RegisterEndpointTests()
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
    public async Task Register_WithValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe",
            PhoneNumber: "+1234567890"
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            request
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var content = await response.Content.ReadAsAsync<RegisterUserResponse>();
        content.Should().NotBeNull();
        content!.Email.Should().Be("john.doe@example.com");
        content.FirstName.Should().Be("John");
        content.LastName.Should().Be("Doe");
        content.Success.Should().BeTrue();
        content.UserId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterUserRequest(
            Email: "invalid-email",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            request
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsAsync<ErrorResponse>();
        content.Should().NotBeNull();
        content!.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_WithWeakPassword_ReturnsBadRequest()
    {
        // Arrange - Password too short, no special character
        var request = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "weak",
            FirstName: "John",
            LastName: "Doe"
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            request
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsAsync<ErrorResponse>();
        content.Should().NotBeNull();
        content!.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_WithMissingFirstName_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "",
            LastName: "Doe"
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            request
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsAsync<ErrorResponse>();
        content.Should().NotBeNull();
    }

    [Fact]
    public async Task Register_WithMissingLastName_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: ""
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            request
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsAsync<ErrorResponse>();
        content.Should().NotBeNull();
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsConflict()
    {
        // Arrange
        var request = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        // Act - First registration
        await _client.PostAsJsonAsync("/api/v1/identity/register", request);

        // Act - Second registration with same email
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            request
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var content = await response.Content.ReadAsAsync<ErrorResponse>();
        content.Should().NotBeNull();
        content!.Message.Should().Contain("already exists");
    }

    [Fact]
    public async Task Register_WithWhitespaceEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterUserRequest(
            Email: "   ",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            request
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithNullPassword_ReturnsBadRequest()
    {
        // Arrange
        var requestJson = """
        {
            "email": "john.doe@example.com",
            "password": null,
            "firstName": "John",
            "lastName": "Doe"
        }
        """;

        var content = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/identity/register", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithPhoneNumber_ReturnsCreatedWithPhoneNumber()
    {
        // Arrange
        var phoneNumber = "+1234567890";
        var request = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe",
            PhoneNumber: phoneNumber
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            request
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseData = await response.Content.ReadAsAsync<RegisterUserResponse>();
        responseData.Should().NotBeNull();
        responseData!.Success.Should().BeTrue();

        // Verify user was created with phone number
        var userId = responseData.UserId;
        var user = await _fixture.GetUserAsync(userId);
        user.Should().NotBeNull();
        user!.PhoneNumber.Should().Be(phoneNumber);
    }

    [Fact]
    public async Task Register_WithoutPhoneNumber_ReturnsCreatedWithoutPhoneNumber()
    {
        // Arrange
        var request = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            request
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseData = await response.Content.ReadAsAsync<RegisterUserResponse>();
        var userId = responseData!.UserId;

        var user = await _fixture.GetUserAsync(userId);
        user.Should().NotBeNull();
        user!.PhoneNumber.Should().BeNullOrEmpty();
    }
}
