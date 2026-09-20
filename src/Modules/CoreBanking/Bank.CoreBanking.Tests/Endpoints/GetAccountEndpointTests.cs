using Bank.CoreBanking.Domain.Entities;
using Bank.CoreBanking.Presentation.Dtos;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Bank.CoreBanking.Tests.Endpoints;

/// <summary>
/// Integration tests for account retrieval endpoints
/// GET /api/v1/core-banking/accounts/{id}
/// GET /api/v1/core-banking/accounts/customer/{customerId}
/// </summary>
[Collection("CoreBanking Integration")]
public class GetAccountEndpointTests : IAsyncLifetime
{
    private readonly CoreBankingTestFixture _fixture;
    private HttpClient _client = null!;
    private Guid _accountId;
    private Guid _customerId;

    public GetAccountEndpointTests()
    {
        _fixture = new CoreBankingTestFixture();
    }

    public async Task InitializeAsync()
    {
        await _fixture.InitializeAsync();
        _client = _fixture.CreateClient();

        // Create test account
        _customerId = Guid.NewGuid();
        var account = Account.Create(
            "ACC123456789ABC",
            "Test Customer",
            _customerId,
            "USD"
        );
        _accountId = account.Id;

        await _fixture.AddAccountAsync(account);
    }

    public async Task DisposeAsync()
    {
        await _fixture.DisposeAsync();
        _client.Dispose();
    }

    [Fact]
    public async Task GetAccount_WithValidId_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync(
            $"/api/v1/core-banking/accounts/{_accountId}"
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsAsync<AccountResponse>();
        content?.Id.Should().Be(_accountId);
        content?.AccountHolderName.Should().Be("Test Customer");
    }

    [Fact]
    public async Task GetAccount_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync(
            $"/api/v1/core-banking/accounts/{nonExistentId}"
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadAsAsync<ErrorResponse>();
        content?.Message.Should().Contain("Account not found");
    }

    [Fact]
    public async Task GetAccount_WithEmptyId_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync(
            $"/api/v1/core-banking/accounts/{Guid.Empty}"
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetCustomerAccounts_WithValidCustomerId_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync(
            $"/api/v1/core-banking/accounts/customer/{_customerId}?pageNumber=1&pageSize=10"
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsAsync<PaginatedResponse<AccountResponse>>();
        content?.Data.Should().ContainSingle();
        content?.Data.First().Id.Should().Be(_accountId);
    }

    [Fact]
    public async Task GetCustomerAccounts_WithEmptyCustomerId_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync(
            $"/api/v1/core-banking/accounts/customer/{Guid.Empty}?pageNumber=1&pageSize=10"
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetCustomerAccounts_WithInvalidPageNumber_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync(
            $"/api/v1/core-banking/accounts/customer/{_customerId}?pageNumber=0&pageSize=10"
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsAsync<ErrorResponse>();
        content?.Message.Should().Contain("PageNumber");
    }

    [Fact]
    public async Task GetCustomerAccounts_WithPageSizeExceedingMax_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync(
            $"/api/v1/core-banking/accounts/customer/{_customerId}?pageNumber=1&pageSize=101"
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsAsync<ErrorResponse>();
        content?.Message.Should().Contain("PageSize");
    }

    [Fact]
    public async Task GetCustomerAccounts_WithPagination_ReturnsPaginationMetadata()
    {
        // Arrange - Create multiple accounts
        for (int i = 0; i < 15; i++)
        {
            var account = Account.Create(
                $"ACC{i:D12}ABC",
                $"Customer {i}",
                _customerId,
                "USD"
            );
            await _fixture.AddAccountAsync(account);
        }

        // Act
        var response = await _client.GetAsync(
            $"/api/v1/core-banking/accounts/customer/{_customerId}?pageNumber=1&pageSize=10"
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsAsync<PaginatedResponse<AccountResponse>>();
        content?.Data.Should().HaveCount(10);
        content?.PageNumber.Should().Be(1);
        content?.PageSize.Should().Be(10);
        content?.TotalCount.Should().Be(16); // 1 from init + 15 created
        content?.TotalPages.Should().Be(2);
        content?.HasNextPage.Should().BeTrue();
        content?.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public async Task GetCustomerAccounts_WithPageTwo_ReturnsRemainingItems()
    {
        // Arrange - Create multiple accounts
        for (int i = 0; i < 15; i++)
        {
            var account = Account.Create(
                $"ACC{i:D12}DEF",
                $"Customer {i}",
                _customerId,
                "USD"
            );
            await _fixture.AddAccountAsync(account);
        }

        // Act
        var response = await _client.GetAsync(
            $"/api/v1/core-banking/accounts/customer/{_customerId}?pageNumber=2&pageSize=10"
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsAsync<PaginatedResponse<AccountResponse>>();
        content?.Data.Should().HaveCount(6); // 1 + 15 - 10 = 6
        content?.PageNumber.Should().Be(2);
        content?.HasPreviousPage.Should().BeTrue();
        content?.HasNextPage.Should().BeFalse();
    }
}
