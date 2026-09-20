namespace Bank.Identity.Tests.Endpoints;

/// <summary>
/// End-to-end authentication flow tests
/// Covers complete user lifecycle: registration, login, profile retrieval, logout
/// </summary>
[Collection("Identity Integration")]
public class AuthenticationFlowE2ETests : IAsyncLifetime
{
    private readonly IdentityTestFixture _fixture;
    private HttpClient _client = null!;

    public AuthenticationFlowE2ETests()
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
    public async Task FullAuthenticationFlow_UserRegistrationLoginGetProfileLogout()
    {
        // Stage 1: Register new user
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

        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var registerData = await registerResponse.Content.ReadAsAsync<RegisterUserResponse>();
        registerData!.Success.Should().BeTrue();
        registerData.Email.Should().Be("john.doe@example.com");
        
        var userId = registerData.UserId;

        // Stage 2: Login with registered credentials
        var loginRequest = new LoginRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!"
        );

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/v1/identity/login",
            loginRequest
        );

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var loginData = await loginResponse.Content.ReadAsAsync<LoginResponse>();
        loginData!.Success.Should().BeTrue();
        loginData.Email.Should().Be("john.doe@example.com");
        loginData.AccessToken.Should().NotBeNullOrEmpty();
        loginData.RefreshToken.Should().NotBeNullOrEmpty();

        var sessionId = loginData.SessionId;
        var accessToken = loginData.AccessToken;

        // Stage 3: Retrieve user profile using retrieved user ID
        var getProfileResponse = await _client.GetAsync($"/api/v1/identity/users/{userId}");

        getProfileResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var profileData = await getProfileResponse.Content.ReadAsAsync<UserResponse>();
        profileData!.Id.Should().Be(userId);
        profileData.Email.Should().Be("john.doe@example.com");
        profileData.FirstName.Should().Be("John");
        profileData.LastName.Should().Be("Doe");
        profileData.FullName.Should().Be("John Doe");
        profileData.PhoneNumber.Should().Be("+1234567890");
        profileData.IsActive.Should().BeTrue();

        // Verify last login was updated
        profileData.LastLoginAtUtc.Should().NotBeNull();

        // Stage 4: Logout
        var logoutRequest = new LogoutUserRequest(sessionId, userId);

        var logoutResponse = await _client.PostAsJsonAsync(
            "/api/v1/identity/logout",
            logoutRequest
        );

        logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify session is terminated in database
        var session = await _fixture.GetSessionAsync(sessionId);
        session!.TerminatedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task MultipleUserRegistrationAndLogin_IndependentSessions()
    {
        // Register User 1
        var user1RegisterRequest = new RegisterUserRequest(
            Email: "user1@example.com",
            Password: "SecurePass123!",
            FirstName: "User",
            LastName: "One"
        );

        var user1RegisterResponse = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            user1RegisterRequest
        );

        var user1Data = await user1RegisterResponse.Content.ReadAsAsync<RegisterUserResponse>();
        var user1Id = user1Data!.UserId;

        // Register User 2
        var user2RegisterRequest = new RegisterUserRequest(
            Email: "user2@example.com",
            Password: "SecurePass456!",
            FirstName: "User",
            LastName: "Two"
        );

        var user2RegisterResponse = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            user2RegisterRequest
        );

        var user2Data = await user2RegisterResponse.Content.ReadAsAsync<RegisterUserResponse>();
        var user2Id = user2Data!.UserId;

        // Login User 1
        var user1LoginRequest = new LoginRequest(
            Email: "user1@example.com",
            Password: "SecurePass123!"
        );

        var user1LoginResponse = await _client.PostAsJsonAsync(
            "/api/v1/identity/login",
            user1LoginRequest
        );

        var user1LoginData = await user1LoginResponse.Content.ReadAsAsync<LoginResponse>();
        var user1SessionId = user1LoginData!.SessionId;

        // Login User 2
        var user2LoginRequest = new LoginRequest(
            Email: "user2@example.com",
            Password: "SecurePass456!"
        );

        var user2LoginResponse = await _client.PostAsJsonAsync(
            "/api/v1/identity/login",
            user2LoginRequest
        );

        var user2LoginData = await user2LoginResponse.Content.ReadAsAsync<LoginResponse>();
        var user2SessionId = user2LoginData!.SessionId;

        // Verify sessions are different
        user1SessionId.Should().NotBe(user2SessionId);

        // Logout User 1
        var user1LogoutRequest = new LogoutUserRequest(user1SessionId, user1Id);
        var user1LogoutResponse = await _client.PostAsJsonAsync(
            "/api/v1/identity/logout",
            user1LogoutRequest
        );

        user1LogoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify User 1's session is terminated
        var user1Session = await _fixture.GetSessionAsync(user1SessionId);
        user1Session!.TerminatedAtUtc.Should().NotBeNull();

        // Verify User 2's session is still active
        var user2Session = await _fixture.GetSessionAsync(user2SessionId);
        user2Session!.TerminatedAtUtc.Should().BeNull();

        // Logout User 2
        var user2LogoutRequest = new LogoutUserRequest(user2SessionId, user2Id);
        var user2LogoutResponse = await _client.PostAsJsonAsync(
            "/api/v1/identity/logout",
            user2LogoutRequest
        );

        user2LogoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var user2SessionAfter = await _fixture.GetSessionAsync(user2SessionId);
        user2SessionAfter!.TerminatedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task UserCanLoginMultipleTimes_CreateMultipleSessions()
    {
        // Register user
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

        var loginRequest = new LoginRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!"
        );

        // Login 3 times
        var sessionIds = new List<Guid>();

        for (int i = 0; i < 3; i++)
        {
            var loginResponse = await _client.PostAsJsonAsync(
                "/api/v1/identity/login",
                loginRequest
            );

            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var loginData = await loginResponse.Content.ReadAsAsync<LoginResponse>();
            sessionIds.Add(loginData!.SessionId);

            await Task.Delay(50); // Small delay between logins
        }

        // Verify all sessions are unique
        sessionIds.Distinct().Count().Should().Be(3);

        // Logout first session
        var logoutRequest1 = new LogoutUserRequest(sessionIds[0], userId);
        var logoutResponse1 = await _client.PostAsJsonAsync(
            "/api/v1/identity/logout",
            logoutRequest1
        );

        logoutResponse1.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify first session is terminated
        var session1 = await _fixture.GetSessionAsync(sessionIds[0]);
        session1!.TerminatedAtUtc.Should().NotBeNull();

        // Verify other sessions are still active
        var session2 = await _fixture.GetSessionAsync(sessionIds[1]);
        session2!.TerminatedAtUtc.Should().BeNull();

        var session3 = await _fixture.GetSessionAsync(sessionIds[2]);
        session3!.TerminatedAtUtc.Should().BeNull();
    }

    [Fact]
    public async Task InvalidLoginAfterRegistration_ThenSuccessfulLogin()
    {
        // Register user
        var registerRequest = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        await _client.PostAsJsonAsync("/api/v1/identity/register", registerRequest);

        // Try login with wrong password
        var wrongLoginRequest = new LoginRequest(
            Email: "john.doe@example.com",
            Password: "WrongPassword123!"
        );

        var wrongLoginResponse = await _client.PostAsJsonAsync(
            "/api/v1/identity/login",
            wrongLoginRequest
        );

        wrongLoginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // Try login with correct password
        var correctLoginRequest = new LoginRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!"
        );

        var correctLoginResponse = await _client.PostAsJsonAsync(
            "/api/v1/identity/login",
            correctLoginRequest
        );

        correctLoginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var loginData = await correctLoginResponse.Content.ReadAsAsync<LoginResponse>();
        loginData!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task RegistrationValidation_MultipleAttempts()
    {
        // Attempt 1: Missing email
        var invalidRequest1 = new RegisterUserRequest(
            Email: "",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        var response1 = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            invalidRequest1
        );

        response1.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Attempt 2: Weak password
        var invalidRequest2 = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "weak",
            FirstName: "John",
            LastName: "Doe"
        );

        var response2 = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            invalidRequest2
        );

        response2.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Attempt 3: Valid registration
        var validRequest = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        var response3 = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            validRequest
        );

        response3.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetUserProfile_VerifyAllDataPersistence()
    {
        // Register with all optional data
        var registerRequest = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe",
            PhoneNumber: "+1-555-0123"
        );

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/v1/identity/register",
            registerRequest
        );

        var registerData = await registerResponse.Content.ReadAsAsync<RegisterUserResponse>();
        var userId = registerData!.UserId;

        // Get profile immediately after registration
        var profileResponse = await _client.GetAsync($"/api/v1/identity/users/{userId}");
        var profileData = await profileResponse.Content.ReadAsAsync<UserResponse>();

        // Verify all data persisted correctly
        profileData!.Id.Should().Be(userId);
        profileData.Email.Should().Be("john.doe@example.com");
        profileData.FirstName.Should().Be("John");
        profileData.LastName.Should().Be("Doe");
        profileData.FullName.Should().Be("John Doe");
        profileData.PhoneNumber.Should().Be("+1-555-0123");
        profileData.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        profileData.IsActive.Should().BeTrue();
    }
}
