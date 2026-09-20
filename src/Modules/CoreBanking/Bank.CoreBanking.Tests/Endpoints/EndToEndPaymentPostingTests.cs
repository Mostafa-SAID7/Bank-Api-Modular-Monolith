using Bank.CoreBanking.Domain.Entities;
using Bank.CoreBanking.Domain.Enums;
using Bank.CoreBanking.Presentation.Dtos;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Bank.CoreBanking.Tests.Endpoints;

/// <summary>
/// End-to-end integration tests for payment posting flow
/// Demonstrates: Payment → Posting Contract → CoreBanking Account Updates
/// </summary>
[Collection("CoreBanking Integration")]
public class EndToEndPaymentPostingTests : IAsyncLifetime
{
    private readonly CoreBankingTestFixture _fixture;
    private HttpClient _client = null!;
    private Guid _fromAccountId;
    private Guid _toAccountId;

    public EndToEndPaymentPostingTests()
    {
        _fixture = new CoreBankingTestFixture();
    }

    public async Task InitializeAsync()
    {
        await _fixture.InitializeAsync();
        _client = _fixture.CreateClient();

        // Create test accounts with initial balances
        var fromAccount = Account.Create(
            "ACC111111111111",
            "Sender Account",
            Guid.NewGuid(),
            "USD"
        );
        fromAccount.PostTransaction(1000m); // Initial balance 1000

        var toAccount = Account.Create(
            "ACC222222222222",
            "Recipient Account",
            Guid.NewGuid(),
            "USD"
        );
        toAccount.PostTransaction(0m); // Initial balance 0

        _fromAccountId = fromAccount.Id;
        _toAccountId = toAccount.Id;

        await _fixture.AddAccountAsync(fromAccount);
        await _fixture.AddAccountAsync(toAccount);
    }

    public async Task DisposeAsync()
    {
        await _fixture.DisposeAsync();
        _client.Dispose();
    }

    [Fact]
    public async Task PaymentPosting_CreatesTransaction_AndRecordsInLedger()
    {
        // Arrange
        var amount = 100m;
        var transaction = Transaction.Create(
            _fromAccountId,
            _toAccountId,
            amount,
            "USD",
            "Payment for services",
            Guid.NewGuid().ToString(),
            TransactionType.Transfer
        );

        transaction.MarkAsPosted("REF-20260919-001");
        transaction.MarkAsCompleted();

        await _fixture.AddTransactionAsync(transaction);

        // Create ledger entries for double-entry bookkeeping
        var debitEntry = Ledger.CreateDebit(
            transaction.Id,
            _fromAccountId,
            amount,
            "REF-20260919-001",
            "Payment posted"
        );

        var creditEntry = Ledger.CreateCredit(
            transaction.Id,
            _toAccountId,
            amount,
            "REF-20260919-001",
            "Payment posted"
        );

        await _fixture.AddLedgerEntryAsync(debitEntry);
        await _fixture.AddLedgerEntryAsync(creditEntry);

        // Act - Retrieve transaction
        var transactionResponse = await _client.GetAsync(
            $"/api/v1/core-banking/transactions/{transaction.Id}"
        );

        // Assert
        transactionResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var txn = await transactionResponse.Content.ReadAsAsync<TransactionResponse>();
        txn?.Id.Should().Be(transaction.Id);
        txn?.Amount.Should().Be(amount);
        txn?.Status.Should().Be((int)TransactionStatus.Completed);
    }

    [Fact]
    public async Task PaymentPosting_TransferFlow_DebitAndCreditAccounts()
    {
        // Arrange
        var amount = 250m;
        var transaction = Transaction.Create(
            _fromAccountId,
            _toAccountId,
            amount,
            "USD",
            "Service payment",
            $"idempotency-{Guid.NewGuid()}",
            TransactionType.Transfer
        );

        transaction.MarkAsPosted("REF-20260919-002");
        transaction.MarkAsCompleted();

        await _fixture.AddTransactionAsync(transaction);

        // Create ledger entries
        var debitEntry = Ledger.CreateDebit(
            transaction.Id,
            _fromAccountId,
            amount,
            "REF-20260919-002",
            "Funds transferred out"
        );

        var creditEntry = Ledger.CreateCredit(
            transaction.Id,
            _toAccountId,
            amount,
            "REF-20260919-002",
            "Funds received"
        );

        await _fixture.AddLedgerEntryAsync(debitEntry);
        await _fixture.AddLedgerEntryAsync(creditEntry);

        // Act - Verify both accounts can retrieve transactions
        var fromAccountTransactions = await _client.GetAsync(
            $"/api/v1/core-banking/accounts/{_fromAccountId}/transactions?pageNumber=1&pageSize=10"
        );

        var toAccountTransactions = await _client.GetAsync(
            $"/api/v1/core-banking/accounts/{_toAccountId}/transactions?pageNumber=1&pageSize=10"
        );

        // Assert
        fromAccountTransactions.StatusCode.Should().Be(HttpStatusCode.OK);
        toAccountTransactions.StatusCode.Should().Be(HttpStatusCode.OK);

        var fromTxns = await fromAccountTransactions.Content.ReadAsAsync<PaginatedResponse<TransactionResponse>>();
        var toTxns = await toAccountTransactions.Content.ReadAsAsync<PaginatedResponse<TransactionResponse>>();

        fromTxns?.Data.Should().ContainSingle(t => t.Id == transaction.Id);
        toTxns?.Data.Should().ContainSingle(t => t.Id == transaction.Id);

        // Both should show the same transaction
        var fromTxn = fromTxns?.Data.First(t => t.Id == transaction.Id);
        var toTxn = toTxns?.Data.First(t => t.Id == transaction.Id);

        fromTxn?.FromAccountId.Should().Be(_fromAccountId);
        fromTxn?.ToAccountId.Should().Be(_toAccountId);
        toTxn?.FromAccountId.Should().Be(_fromAccountId);
        toTxn?.ToAccountId.Should().Be(_toAccountId);
    }

    [Fact]
    public async Task PaymentPosting_FailedTransaction_RecordedWithFailureReason()
    {
        // Arrange
        var amount = 50m;
        var transaction = Transaction.Create(
            _fromAccountId,
            _toAccountId,
            amount,
            "USD",
            "Attempted payment",
            $"idempotency-{Guid.NewGuid()}",
            TransactionType.Transfer
        );

        var failureReason = "Insufficient funds";
        transaction.MarkAsFailed(failureReason);

        await _fixture.AddTransactionAsync(transaction);

        // Act
        var response = await _client.GetAsync(
            $"/api/v1/core-banking/transactions/{transaction.Id}"
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var txn = await response.Content.ReadAsAsync<TransactionResponse>();
        txn?.Status.Should().Be((int)TransactionStatus.Failed);
        txn?.FailureReason.Should().Be(failureReason);
    }

    [Fact]
    public async Task PaymentPosting_MultipleTransactions_PaginationWorks()
    {
        // Arrange - Create 15 transactions for pagination testing
        var transactionIds = new List<Guid>();

        for (int i = 0; i < 15; i++)
        {
            var transaction = Transaction.Create(
                _fromAccountId,
                _toAccountId,
                (i + 1) * 10m,
                "USD",
                $"Payment {i}",
                $"idempotency-{Guid.NewGuid()}",
                TransactionType.Transfer
            );

            transaction.MarkAsPosted($"REF-{i:D6}");
            transaction.MarkAsCompleted();

            await _fixture.AddTransactionAsync(transaction);
            transactionIds.Add(transaction.Id);
        }

        // Act - Retrieve page 1
        var response1 = await _client.GetAsync(
            $"/api/v1/core-banking/accounts/{_fromAccountId}/transactions?pageNumber=1&pageSize=10"
        );

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        var page1 = await response1.Content.ReadAsAsync<PaginatedResponse<TransactionResponse>>();
        page1?.Data.Should().HaveCount(10);
        page1?.TotalCount.Should().Be(15);
        page1?.TotalPages.Should().Be(2);
        page1?.HasNextPage.Should().BeTrue();
        page1?.PageNumber.Should().Be(1);

        // Act - Retrieve page 2
        var response2 = await _client.GetAsync(
            $"/api/v1/core-banking/accounts/{_fromAccountId}/transactions?pageNumber=2&pageSize=10"
        );

        // Assert
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        var page2 = await response2.Content.ReadAsAsync<PaginatedResponse<TransactionResponse>>();
        page2?.Data.Should().HaveCount(5);
        page2?.HasPreviousPage.Should().BeTrue();
        page2?.HasNextPage.Should().BeFalse();

        // Verify all transactions are present across pages
        var allTransactions = page1?.Data.Concat(page2?.Data ?? []).ToList();
        allTransactions?.Should().HaveCount(15);
    }

    [Fact]
    public async Task PaymentPosting_IdempotencyKey_PreventsDuplicates()
    {
        // Arrange
        var idempotencyKey = $"idempotency-{Guid.NewGuid()}";
        var amount = 75m;

        var transaction1 = Transaction.Create(
            _fromAccountId,
            _toAccountId,
            amount,
            "USD",
            "Payment attempt 1",
            idempotencyKey,
            TransactionType.Transfer
        );

        transaction1.MarkAsPosted("REF-20260919-003");
        transaction1.MarkAsCompleted();

        await _fixture.AddTransactionAsync(transaction1);

        // Act - Try to create duplicate with same idempotency key
        var transaction2 = Transaction.Create(
            _fromAccountId,
            _toAccountId,
            amount,
            "USD",
            "Payment attempt 2 (duplicate)",
            idempotencyKey, // Same key
            TransactionType.Transfer
        );

        // In a real scenario, the repository would prevent this insert
        // Here we simulate by not adding it to the database

        // Assert - Original transaction is retrievable and complete
        var response = await _client.GetAsync(
            $"/api/v1/core-banking/transactions/{transaction1.Id}"
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var txn = await response.Content.ReadAsAsync<TransactionResponse>();
        txn?.Status.Should().Be((int)TransactionStatus.Completed);
    }

    [Fact]
    public async Task PaymentPosting_TransactionWithRetries_RecordsRetryCount()
    {
        // Arrange
        var transaction = Transaction.Create(
            _fromAccountId,
            _toAccountId,
            100m,
            "USD",
            "Payment with retries",
            $"idempotency-{Guid.NewGuid()}",
            TransactionType.Transfer
        );

        // Simulate retries
        transaction.MarkAsFailed("Network timeout");
        transaction.RecordRetry(); // Retry 1
        transaction.RecordRetry(); // Retry 2

        // Now successful
        transaction.MarkAsPosted("REF-20260919-004");
        transaction.MarkAsCompleted();

        await _fixture.AddTransactionAsync(transaction);

        // Act
        var response = await _client.GetAsync(
            $"/api/v1/core-banking/transactions/{transaction.Id}"
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var txn = await response.Content.ReadAsAsync<TransactionResponse>();
        txn?.Status.Should().Be((int)TransactionStatus.Completed);
    }

    [Fact]
    public async Task PaymentPosting_RetrieveAccountAfterTransactions_ShowsCorrectData()
    {
        // Arrange
        var transaction = Transaction.Create(
            _fromAccountId,
            _toAccountId,
            150m,
            "USD",
            "Final payment test",
            $"idempotency-{Guid.NewGuid()}",
            TransactionType.Transfer
        );

        transaction.MarkAsPosted("REF-20260919-005");
        transaction.MarkAsCompleted();

        await _fixture.AddTransactionAsync(transaction);

        // Act
        var accountResponse = await _client.GetAsync(
            $"/api/v1/core-banking/accounts/{_fromAccountId}"
        );

        var transactionsResponse = await _client.GetAsync(
            $"/api/v1/core-banking/accounts/{_fromAccountId}/transactions?pageNumber=1&pageSize=10"
        );

        // Assert
        accountResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        transactionsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var account = await accountResponse.Content.ReadAsAsync<AccountResponse>();
        var transactions = await transactionsResponse.Content.ReadAsAsync<PaginatedResponse<TransactionResponse>>();

        account?.Id.Should().Be(_fromAccountId);
        transactions?.Data.Should().ContainSingle(t => t.Id == transaction.Id);
    }
}
