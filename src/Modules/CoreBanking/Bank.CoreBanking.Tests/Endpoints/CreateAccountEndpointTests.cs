using Bank.CoreBanking.Presentation.Dtos;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Bank.CoreBanking.Tests.Endpoints;

/// <summary>
/// Integration tests for account creation endpoint
/// POST /api/v1/core-banking/accounts
/// </summary>
[Collection("CoreBanking Integration")]
public class CreateAccountEndpointTests : IAsyncLifetime
{
    private readonly CoreBankingTestFixture _fixture;
    private HttpClient _client = null!;

    public CreateAccountEndpointTests()
    {
        _fixture = new CoreBankingTestFixture();
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
    public async Task CreateAccount_WithValidRequest_ReturnsCreated()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var request = new CreateAccountRequest(
            AccountHolderName: "John Doe",
            CustomerId: customerId,
            Currency: "USD",
            Type: 0
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/core-banking/accounts",
            request
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var content = await response.Content.ReadAsAsync<AccountResponse>();
        content.Should().NotBeNull();
        content!.AccountHolderName.Should().Be("John Doe");
        content.CustomerId.Should().Be(customerId);
        content.Currency.Should().Be("USD");
        content.Balance.Should().Be(0m);
        content.Status.Should().Be(0); // Active
        content.Type.Should().Be(0); // Checking
    }

    [Fact]
    public async Task CreateAccount_WithMissingAccountHolderName_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateAccountRequest(
            AccountHolderName: "",
            CustomerId: Guid.NewGuid(),
            Currency: "USD",
            Type: 0
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/core-banking/accounts",
            request
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsAsync<ErrorResponse>();
        content?.Message.Should().Contain("AccountHolderName");
    }

    [Fact]
    public async Task CreateAccount_WithInvalidCurrency_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateAccountRequest(
            AccountHolderName: "John Doe",
            CustomerId: Guid.NewGuid(),
            Currency: "INVALID",
            Type: 0
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/core-banking/accounts",
            request
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsAsync<ErrorResponse>();
        content?.Message.Should().Contain("Currency");
    }

    [Fact]
    public async Task CreateAccount_WithEmptyCustomerId_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateAccountRequest(
            AccountHolderName: "John Doe",
            CustomerId: Guid.Empty,
            Currency: "USD",
            Type: 0
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/core-banking/accounts",
            request
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsAsync<ErrorResponse>();
        content?.Message.Should().Contain("CustomerId");
    }

    [Fact]
    public async Task CreateAccount_WithExcessiveAccountHolderName_ReturnsBadRequest()
    {
        // Arrange
        var longName = new string('A', 101); // Exceeds max length of 100
        var request = new CreateAccountRequest(
            AccountHolderName: longName,
            CustomerId: Guid.NewGuid(),
            Currency: "USD",
            Type: 0
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/core-banking/accounts",
            request
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsAsync<ErrorResponse>();
        content?.Message.Should().Contain("AccountHolderName");
    }

    [Fact]
    public async Task CreateAccount_GeneratedAccountNumberIsUnique()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var request1 = new CreateAccountRequest(
            AccountHolderName: "Customer 1",
            CustomerId: customerId,
            Currency: "USD",
            Type: 0
        );

        // Act - Create first account
        var response1 = await _client.PostAsJsonAsync(
            "/api/v1/core-banking/accounts",
            request1
        );
        var account1 = await response1.Content.ReadAsAsync<AccountResponse>();

        // Create second account
        var request2 = new CreateAccountRequest(
            AccountHolderName: "Customer 2",
            CustomerId: Guid.NewGuid(),
            Currency: "USD",
            Type: 0
        );
        var response2 = await _client.PostAsJsonAsync(
            "/api/v1/core-banking/accounts",
            request2
        );
        var account2 = await response2.Content.ReadAsAsync<AccountResponse>();

        // Assert
        account1!.AccountNumber.Should().NotBe(account2!.AccountNumber);
    }
}
