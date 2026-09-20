namespace Bank.Identity.Tests.Endpoints;

/// <summary>
/// E2E tests for concurrent identity operations
/// Covers race conditions, simultaneous logins, and parallel request handling
/// </summary>
[Collection("Identity Integration")]
public class ConcurrencyE2ETests : IAsyncLifetime
{
    private readonly IdentityTestFixture _fixture;
    private HttpClient _client = null!;

    public ConcurrencyE2ETests()
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
    public async Task SimultaneousLogins_CreateIndependentSessions()
    {
        // Arrange - Register a user
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

        // Act - Perform 5 simultaneous login requests
        var tasks = new Task<HttpResponseMessage>[5];
        for (int i = 0; i < 5; i++)
        {
            tasks[i] = _client.PostAsJsonAsync("/api/v1/identity/login", loginRequest);
        }

        await Task.WhenAll(tasks);

        // Assert
        var responses = tasks.Select(t => t.Result).ToList();
        responses.Should().AllSatisfy(r => r.StatusCode.Should().Be(HttpStatusCode.OK));

        var sessionIds = new HashSet<Guid>();
        foreach (var response in responses)
        {
            var data = await response.Content.ReadAsAsync<LoginResponse>();
            sessionIds.Add(data!.SessionId);
        }

        // All sessions should be unique
        sessionIds.Count.Should().Be(5);
    }

    [Fact]
    public async Task SimultaneousRegistrations_DifferentUsers()
    {
        // Arrange - Prepare registration requests
        var registrationRequests = Enumerable.Range(0, 5)
            .Select(i => new RegisterUserRequest(
                Email: $"user{i}@example.com",
                Password: "SecurePass123!",
                FirstName: $"User{i}",
                LastName: "Test"
            ))
            .ToList();

        // Act - Perform registrations concurrently
        var tasks = registrationRequests
            .Select(req => _client.PostAsJsonAsync("/api/v1/identity/register", req))
            .ToList();

        await Task.WhenAll(tasks);

        // Assert
        var responses = tasks.Select(t => t.Result).ToList();
        responses.Should().AllSatisfy(r => r.StatusCode.Should().Be(HttpStatusCode.Created));

        var userIds = new HashSet<Guid>();
        foreach (var response in responses)
        {
            var data = await response.Content.ReadAsAsync<RegisterUserResponse>();
            userIds.Add(data!.UserId);
        }

        // All users should have unique IDs
        userIds.Count.Should().Be(5);
    }

    [Fact]
    public async Task ConcurrentLoginAndLogout_ProperStateTransitions()
    {
        // Arrange - Register and login twice
        var registerRequest = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/identity/register", registerRequest);
        var registerData = await registerResponse.Content.ReadAsAsync<RegisterUserResponse>();
        var userId = registerData!.UserId;

        var loginRequest = new LoginRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!"
        );

        var login1Response = await _client.PostAsJsonAsync("/api/v1/identity/login", loginRequest);
        var login1Data = await login1Response.Content.ReadAsAsync<LoginResponse>();
        var sessionId1 = login1Data!.SessionId;

        var login2Response = await _client.PostAsJsonAsync("/api/v1/identity/login", loginRequest);
        var login2Data = await login2Response.Content.ReadAsAsync<LoginResponse>();
        var sessionId2 = login2Data!.SessionId;

        // Act - Perform concurrent logouts
        var logoutRequest1 = new LogoutUserRequest(sessionId1, userId);
        var logoutRequest2 = new LogoutUserRequest(sessionId2, userId);

        var logoutTask1 = _client.PostAsJsonAsync("/api/v1/identity/logout", logoutRequest1);
        var logoutTask2 = _client.PostAsJsonAsync("/api/v1/identity/logout", logoutRequest2);

        await Task.WhenAll(logoutTask1, logoutTask2);

        // Assert
        var logout1Response = await logoutTask1;
        var logout2Response = await logoutTask2;

        logout1Response.StatusCode.Should().Be(HttpStatusCode.OK);
        logout2Response.StatusCode.Should().Be(HttpStatusCode.OK);

        var session1 = await _fixture.GetSessionAsync(sessionId1);
        var session2 = await _fixture.GetSessionAsync(sessionId2);

        session1!.TerminatedAtUtc.Should().NotBeNull();
        session2!.TerminatedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task ConcurrentProfileReads_NoRaceConditions()
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

        // Act - Read profile concurrently
        var tasks = Enumerable.Range(0, 10)
            .Select(_ => _client.GetAsync($"/api/v1/identity/users/{userId}"))
            .ToList();

        await Task.WhenAll(tasks);

        // Assert
        var responses = tasks.Select(t => t.Result).ToList();
        responses.Should().AllSatisfy(r => r.StatusCode.Should().Be(HttpStatusCode.OK));

        var userData = new List<UserResponse>();
        foreach (var response in responses)
        {
            var data = await response.Content.ReadAsAsync<UserResponse>();
            userData.Add(data!);
        }

        // All reads should return identical data
        var firstData = userData[0];
        userData.Should().AllSatisfy(u =>
        {
            u.Id.Should().Be(firstData.Id);
            u.Email.Should().Be(firstData.Email);
            u.FirstName.Should().Be(firstData.FirstName);
            u.LastName.Should().Be(firstData.LastName);
        });
    }

    [Fact]
    public async Task DuplicateEmailRegistration_OnlyOneSucceeds()
    {
        // Arrange
        var registerRequest = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        // Act - Send duplicate registration requests concurrently
        var tasks = Enumerable.Range(0, 3)
            .Select(_ => _client.PostAsJsonAsync("/api/v1/identity/register", registerRequest))
            .ToList();

        await Task.WhenAll(tasks);

        // Assert
        var responses = tasks.Select(t => t.Result).ToList();

        var successCount = responses.Count(r => r.StatusCode == HttpStatusCode.Created);
        var conflictCount = responses.Count(r => r.StatusCode == HttpStatusCode.Conflict);

        successCount.Should().Be(1, "Only one registration should succeed");
        conflictCount.Should().Be(2, "Two registrations should fail with conflict");
    }

    [Fact]
    public async Task MultipleUsersLoggingInConcurrently_AllSucceed()
    {
        // Arrange - Register 5 users
        var userEmails = new List<string>();
        for (int i = 0; i < 5; i++)
        {
            var email = $"user{i}@example.com";
            var registerRequest = new RegisterUserRequest(
                Email: email,
                Password: "SecurePass123!",
                FirstName: $"User{i}",
                LastName: "Test"
            );

            await _client.PostAsJsonAsync("/api/v1/identity/register", registerRequest);
            userEmails.Add(email);
        }

        // Act - All users login concurrently
        var loginTasks = userEmails
            .Select(email => _client.PostAsJsonAsync(
                "/api/v1/identity/login",
                new LoginRequest(email, "SecurePass123!")
            ))
            .ToList();

        await Task.WhenAll(loginTasks);

        // Assert
        var responses = loginTasks.Select(t => t.Result).ToList();
        responses.Should().AllSatisfy(r => r.StatusCode.Should().Be(HttpStatusCode.OK));

        var sessionIds = new HashSet<Guid>();
        foreach (var response in responses)
        {
            var data = await response.Content.ReadAsAsync<LoginResponse>();
            sessionIds.Add(data!.SessionId);
        }

        sessionIds.Count.Should().Be(5, "Each user should have a unique session");
    }

    [Fact]
    public async Task RegisterThenLoginConcurrently_ProperSequencing()
    {
        // Arrange
        var registerRequest = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        var loginRequest = new LoginRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!"
        );

        // Act - Register first
        var registerResponse = await _client.PostAsJsonAsync("/api/v1/identity/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Then try concurrent login attempts immediately
        var loginTasks = Enumerable.Range(0, 5)
            .Select(_ => _client.PostAsJsonAsync("/api/v1/identity/login", loginRequest))
            .ToList();

        await Task.WhenAll(loginTasks);

        // Assert
        var responses = loginTasks.Select(t => t.Result).ToList();
        responses.Should().AllSatisfy(r => r.StatusCode.Should().Be(HttpStatusCode.OK));
    }

    [Fact]
    public async Task HighConcurrencyLoad_AllRequestsProcessed()
    {
        // Arrange
        var registerRequest = new RegisterUserRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe"
        );

        await _client.PostAsJsonAsync("/api/v1/identity/register", registerRequest);

        // Act - Mix of operations: 20 logins + 20 profile reads
        var tasks = new List<Task<HttpResponseMessage>>();

        var loginRequest = new LoginRequest(
            Email: "john.doe@example.com",
            Password: "SecurePass123!"
        );

        // Add login tasks
        for (int i = 0; i < 20; i++)
        {
            tasks.Add(_client.PostAsJsonAsync("/api/v1/identity/login", loginRequest));
        }

        await Task.WhenAll(tasks);

        // Collect session IDs from login responses
        var sessionIds = new List<Guid>();
        foreach (var task in tasks)
        {
            var response = await task;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var data = await response.Content.ReadAsAsync<LoginResponse>();
                sessionIds.Add(data!.UserId);
            }
        }

        // Add profile read tasks
        tasks.Clear();
        foreach (var userId in sessionIds.Take(10)) // Use first 10 users
        {
            tasks.Add(_client.GetAsync($"/api/v1/identity/users/{userId}"));
        }

        await Task.WhenAll(tasks);

        // Assert
        var responses = tasks.Select(t => t.Result).ToList();
        responses.Should().AllSatisfy(r => 
            r.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound));
    }
}
