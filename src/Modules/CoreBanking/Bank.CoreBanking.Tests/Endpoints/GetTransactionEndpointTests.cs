using Bank.CoreBanking.Domain.Entities;
using Bank.CoreBanking.Domain.Enums;
using Bank.CoreBanking.Presentation.Dtos;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Bank.CoreBanking.Tests.Endpoints;

/// <summary>
/// Integration tests for transaction retrieval endpoints
/// GET /api/v1/core-banking/transactions/{id}
/// GET /api/v1/core-banking/accounts/{accountId}/transactions
/// </summary>
[Collection("CoreBanking Integration")]
public class GetTransactionEndpointTests : IAsyncLifetime
{
    private readonly CoreBankingTestFixture _fixture;
    private HttpClient _client = null!;
    private Guid _transactionId;
    private Guid _accountId;

    public GetTransactionEndpointTests()
    {
        _fixture = new CoreBankingTestFixture();
    }

    public async Task InitializeAsync()
    {
        await _fixture.InitializeAsync();
        _client = _fixture.CreateClient();

        // Create test accounts and transaction
        var fromAccount = Account.Create("ACC111111111111", "From", Guid.NewGuid(), "USD");
        var toAccount = Account.Create("ACC222222222222", "To", Guid.NewGuid(), "USD");
        _accountId = fromAccount.Id;

        await _fixture.AddAccountAsync(fromAccount);
        await _fixture.AddAccountAsync(toAccount);

        var transaction = Transaction.Create(
            fromAccount.Id,
            toAccount.Id,
            100m,
            "USD",
            "Test transfer",
            Guid.NewGuid().ToString(),
            TransactionType.Transfer
        );
        _transactionId = transaction.Id;

        await _fixture.AddTransactionAsync(transaction);
    }

    public async Task DisposeAsync()
    {
        await _fixture.DisposeAsync();
        _client.Dispose();
    }

    [Fact]
    public async Task GetTransaction_WithValidId_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync(
            $"/api/v1/core-banking/transactions/{_transactionId}"
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsAsync<TransactionResponse>();
        content?.Id.Should().Be(_transactionId);
        content?.Amount.Should().Be(100m);
        content?.Currency.Should().Be("USD");
    }

    [Fact]
    public async Task GetTransaction_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync(
            $"/api/v1/core-banking/transactions/{nonExistentId}"
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadAsAsync<ErrorResponse>();
        content?.Message.Should().Contain("Transaction not found");
    }

    [Fact]
    public async Task GetAccountTransactions_WithValidAccountId_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync(
            $"/api/v1/core-banking/accounts/{_accountId}/transactions?pageNumber=1&pageSize=10"
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsAsync<PaginatedResponse<TransactionResponse>>();
        content?.Data.Should().ContainSingle();
        content?.Data.First().Id.Should().Be(_transactionId);
    }

    [Fact]
    public async Task GetAccountTransactions_WithEmptyAccountId_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync(
            $"/api/v1/core-banking/accounts/{Guid.Empty}/transactions?pageNumber=1&pageSize=10"
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAccountTransactions_WithInvalidPageNumber_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync(
            $"/api/v1/core-banking/accounts/{_accountId}/transactions?pageNumber=0&pageSize=10"
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsAsync<ErrorResponse>();
        content?.Message.Should().Contain("PageNumber");
    }

    [Fact]
    public async Task GetAccountTransactions_WithPageSizeExceedingMax_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync(
            $"/api/v1/core-banking/accounts/{_accountId}/transactions?pageNumber=1&pageSize=101"
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsAsync<ErrorResponse>();
        content?.Message.Should().Contain("PageSize");
    }

    [Fact]
    public async Task GetAccountTransactions_WithPagination_ReturnsPaginationMetadata()
    {
        // Arrange - Create multiple transactions
        var toAccount = Account.Create("ACC333333333333", "To Multi", Guid.NewGuid(), "USD");
        await _fixture.AddAccountAsync(toAccount);

        for (int i = 0; i < 15; i++)
        {
            var transaction = Transaction.Create(
                _accountId,
                toAccount.Id,
                (i + 1) * 10m,
                "USD",
                $"Transfer {i}",
                $"idempotency-{Guid.NewGuid()}",
                TransactionType.Transfer
            );
            await _fixture.AddTransactionAsync(transaction);
        }

        // Act
        var response = await _client.GetAsync(
            $"/api/v1/core-banking/accounts/{_accountId}/transactions?pageNumber=1&pageSize=10"
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsAsync<PaginatedResponse<TransactionResponse>>();
        content?.Data.Should().HaveCount(10);
        content?.PageNumber.Should().Be(1);
        content?.PageSize.Should().Be(10);
        content?.TotalCount.Should().Be(16); // 1 from init + 15 created
        content?.TotalPages.Should().Be(2);
        content?.HasNextPage.Should().BeTrue();
        content?.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public async Task GetAccountTransactions_ReturnsTransactionsSortedByMostRecentFirst()
    {
        // Arrange - Get initial transaction creation time
        var initialResponse = await _client.GetAsync(
            $"/api/v1/core-banking/accounts/{_accountId}/transactions?pageNumber=1&pageSize=10"
        );
        var initialData = await initialResponse.Content.ReadAsAsync<PaginatedResponse<TransactionResponse>>();
        var initialCreatedTime = initialData?.Data.First().CreatedAtUtc;

        // Create a newer transaction
        var toAccount = Account.Create("ACC444444444444", "To Sort", Guid.NewGuid(), "USD");
        await _fixture.AddAccountAsync(toAccount);

        var newerTransaction = Transaction.Create(
            _accountId,
            toAccount.Id,
            50m,
            "USD",
            "Newer transfer",
            $"idempotency-{Guid.NewGuid()}",
            TransactionType.Transfer
        );
        await _fixture.AddTransactionAsync(newerTransaction);

        // Act
        var response = await _client.GetAsync(
            $"/api/v1/core-banking/accounts/{_accountId}/transactions?pageNumber=1&pageSize=10"
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsAsync<PaginatedResponse<TransactionResponse>>();
        content?.Data.First().Id.Should().Be(newerTransaction.Id); // Most recent should be first
        content?.Data.Last().CreatedAtUtc.Should().BeLessThanOrEqualTo(content?.Data.First().CreatedAtUtc);
    }
}
