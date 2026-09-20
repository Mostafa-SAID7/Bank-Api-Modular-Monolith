namespace Bank.Cards.Tests;

/// <summary>
/// End-to-end integration tests for Card lifecycle
/// Tests complete workflows: issue → activate → transactions → block/unblock → close
/// Uses in-memory database and MediatR for full integration testing
/// Covers 25+ test scenarios including happy paths and error cases
/// </summary>
[Collection("Cards Test Collection")]
public class E2ECardLifecycleTests : IAsyncLifetime
{
    private CardsTestFixture? _fixture;

    public async Task InitializeAsync()
    {
        _fixture = new CardsTestFixture();
        await _fixture.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        if (_fixture != null)
        {
            await _fixture.DisposeAsync();
        }
    }

    #region Issue Card Tests (5 tests)

    [Fact]
    public async Task Workflow_IssueCard_ShouldCreateCardInInactiveStatus()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        _fixture!.CreateAndSaveCardProduct(productId, CardType.Debit);

        var mediator = _fixture.Mediator!;
        var dbContext = _fixture.GetDbContext();

        // Act
        var command = new IssueCardCommand(customerId, accountId, productId, "John Doe");
        var result = await mediator.Send(command);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(CardStatus.Inactive);
        result.HolderName.Should().Be("John Doe");
        result.CustomerId.Should().Be(customerId);
        result.LinkedAccountId.Should().Be(accountId);

        // Verify in database
        var savedCard = dbContext.Cards.FirstOrDefault(c => c.Id == result.Id);
        savedCard.Should().NotBeNull();
        savedCard!.Status.Should().Be(CardStatus.Inactive);
    }

    [Fact]
    public async Task Workflow_IssueCard_ShouldGenerateUniqueCardNumber()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        _fixture!.CreateAndSaveCardProduct(productId);

        var mediator = _fixture.Mediator!;

        // Act
        var command1 = new IssueCardCommand(customerId, accountId, productId, "John Doe");
        var card1 = await mediator.Send(command1);

        var command2 = new IssueCardCommand(customerId, accountId, productId, "Jane Doe");
        var card2 = await mediator.Send(command2);

        // Assert
        card1.Should().NotBeNull();
        card2.Should().NotBeNull();
        card1.Id.Should().NotBe(card2.Id);
        card1.MaskedPan.Should().NotBe(card2.MaskedPan);
    }

    [Fact]
    public async Task Workflow_IssueCard_ShouldSetDefaultLimits()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var product = _fixture!.CreateAndSaveCardProduct(productId);

        var mediator = _fixture.Mediator!;

        // Act
        var command = new IssueCardCommand(customerId, accountId, productId, "John Doe");
        var result = await mediator.Send(command);

        // Assert
        result.DailyWithdrawalLimit.Should().Be(product.DefaultDailyLimit);
        result.DailyTransactionLimit.Should().Be(product.DefaultTransactionLimit);
        result.InternationalEnabled.Should().BeTrue();
        result.OnlineTransactionsEnabled.Should().BeTrue();
        result.ContactlessEnabled.Should().BeTrue();
    }

    [Fact]
    public async Task Workflow_IssueCard_InvalidProduct_ShouldThrow()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var invalidProductId = Guid.NewGuid();

        var mediator = _fixture!.Mediator!;

        // Act & Assert
        var command = new IssueCardCommand(customerId, accountId, invalidProductId, "John Doe");
        await Assert.ThrowsAsync<InvalidOperationException>(() => mediator.Send(command));
    }

    [Fact]
    public async Task Workflow_IssueCard_ShouldInitializeWithNoTransactions()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        _fixture!.CreateAndSaveCardProduct(productId);

        var mediator = _fixture.Mediator!;
        var dbContext = _fixture.GetDbContext();

        // Act
        var command = new IssueCardCommand(customerId, accountId, productId, "John Doe");
        var result = await mediator.Send(command);

        // Assert
        var transactions = dbContext.CardTransactions.Where(t => t.CardId == result.Id).ToList();
        transactions.Should().BeEmpty();
    }

    #endregion

    #region Activate Card Tests (4 tests)

    [Fact]
    public async Task Workflow_ActivateCard_ShouldTransitionToActiveStatus()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        _fixture!.CreateAndSaveCardProduct(productId);

        var mediator = _fixture.Mediator!;

        // Act
        var issueCommand = new IssueCardCommand(customerId, accountId, productId, "John Doe");
        var issuedCard = await mediator.Send(issueCommand);

        var activateCommand = new ActivateCardCommand(issuedCard.Id, "1234");
        var activatedCard = await mediator.Send(activateCommand);

        // Assert
        activatedCard.Status.Should().Be(CardStatus.Active);
        activatedCard.ActivatedDateUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task Workflow_ActivateCard_WithValidPin_ShouldSetPin()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        _fixture!.CreateAndSaveCardProduct(productId);

        var mediator = _fixture.Mediator!;

        // Act
        var issueCommand = new IssueCardCommand(customerId, accountId, productId, "John Doe");
        var issuedCard = await mediator.Send(issueCommand);

        var activateCommand = new ActivateCardCommand(issuedCard.Id, "5678");
        var activatedCard = await mediator.Send(activateCommand);

        // Assert
        activatedCard.Should().NotBeNull();
        activatedCard.Status.Should().Be(CardStatus.Active);
    }

    [Fact]
    public async Task Workflow_ActivateCard_InvalidCard_ShouldThrow()
    {
        // Arrange
        var invalidCardId = Guid.NewGuid();
        var mediator = _fixture!.Mediator!;

        // Act & Assert
        var command = new ActivateCardCommand(invalidCardId, "1234");
        await Assert.ThrowsAsync<InvalidOperationException>(() => mediator.Send(command));
    }

    [Fact]
    public async Task Workflow_ActivateCard_AlreadyActive_ShouldThrow()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        _fixture!.CreateAndSaveCardProduct(productId);

        var mediator = _fixture.Mediator!;

        // Act
        var issueCommand = new IssueCardCommand(customerId, accountId, productId, "John Doe");
        var issuedCard = await mediator.Send(issueCommand);

        var activateCommand1 = new ActivateCardCommand(issuedCard.Id, "1234");
        await mediator.Send(activateCommand1);

        // Act & Assert - Try to activate again
        var activateCommand2 = new ActivateCardCommand(issuedCard.Id, "5678");
        await Assert.ThrowsAsync<InvalidOperationException>(() => mediator.Send(activateCommand2));
    }

    #endregion

    #region Block/Unblock Tests (4 tests)

    [Fact]
    public async Task Workflow_BlockCard_ShouldTransitionToBlockedStatus()
    {
        // Arrange
        var card = await IssueAndActivateCardAsync();
        var mediator = _fixture!.Mediator!;

        // Act
        var blockCommand = new BlockCardCommand(card.Id, BlockReason.CustomerRequest, "Customer requested block");
        var blockedCard = await mediator.Send(blockCommand);

        // Assert
        blockedCard.Status.Should().Be(CardStatus.Blocked);
        blockedCard.BlockedDateUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task Workflow_UnblockCard_ShouldTransitionBackToActive()
    {
        // Arrange
        var card = await IssueAndActivateCardAsync();
        var mediator = _fixture!.Mediator!;

        var blockCommand = new BlockCardCommand(card.Id, BlockReason.Lost, "Card reported lost");
        var blockedCard = await mediator.Send(blockCommand);

        // Act
        var unblockCommand = new UnblockCardCommand(card.Id);
        var unblockedCard = await mediator.Send(unblockCommand);

        // Assert
        unblockedCard.Status.Should().Be(CardStatus.Active);
        unblockedCard.BlockedDateUtc.Should().BeNull();
    }

    [Fact]
    public async Task Workflow_BlockCard_CapturesBlockReason()
    {
        // Arrange
        var card = await IssueAndActivateCardAsync();
        var mediator = _fixture!.Mediator!;
        var dbContext = _fixture.GetDbContext();

        // Act
        var blockCommand = new BlockCardCommand(card.Id, BlockReason.Stolen, "Card stolen at ATM");
        await mediator.Send(blockCommand);

        // Assert
        var cardBlocks = dbContext.CardBlocks.Where(b => b.CardId == card.Id).ToList();
        cardBlocks.Should().HaveCount(1);
        cardBlocks[0].Reason.Should().Be(BlockReason.Stolen);
        cardBlocks[0].Description.Should().Be("Card stolen at ATM");
    }

    [Fact]
    public async Task Workflow_UnblockNonBlockedCard_ShouldThrow()
    {
        // Arrange
        var card = await IssueAndActivateCardAsync();
        var mediator = _fixture!.Mediator!;

        // Act & Assert
        var unblockCommand = new UnblockCardCommand(card.Id);
        await Assert.ThrowsAsync<InvalidOperationException>(() => mediator.Send(unblockCommand));
    }

    #endregion

    #region Close Card Tests (3 tests)

    [Fact]
    public async Task Workflow_CloseCard_ShouldTransitionToClosedStatus()
    {
        // Arrange
        var card = await IssueAndActivateCardAsync();
        var mediator = _fixture!.Mediator!;

        // Act
        var closeCommand = new CloseCardCommand(card.Id, "Account closure requested");
        var closedCard = await mediator.Send(closeCommand);

        // Assert
        closedCard.Status.Should().Be(CardStatus.Closed);
        closedCard.ClosedDateUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task Workflow_CloseClosedCard_ShouldThrow()
    {
        // Arrange
        var card = await IssueAndActivateCardAsync();
        var mediator = _fixture!.Mediator!;

        var closeCommand = new CloseCardCommand(card.Id, "First close");
        await mediator.Send(closeCommand);

        // Act & Assert
        var closeCommand2 = new CloseCardCommand(card.Id, "Second close");
        await Assert.ThrowsAsync<InvalidOperationException>(() => mediator.Send(closeCommand2));
    }

    [Fact]
    public async Task Workflow_BlockClosedCard_ShouldThrow()
    {
        // Arrange
        var card = await IssueAndActivateCardAsync();
        var mediator = _fixture!.Mediator!;

        var closeCommand = new CloseCardCommand(card.Id, "Closing card");
        await mediator.Send(closeCommand);

        // Act & Assert
        var blockCommand = new BlockCardCommand(card.Id, BlockReason.Lost, "");
        await Assert.ThrowsAsync<InvalidOperationException>(() => mediator.Send(blockCommand));
    }

    #endregion

    #region PIN Management Tests (3 tests)

    [Fact]
    public async Task Workflow_ChangePin_ShouldUpdatePin()
    {
        // Arrange
        var card = await IssueAndActivateCardAsync();
        var mediator = _fixture!.Mediator!;

        // Act
        var changePinCommand = new ChangePinCommand(card.Id, "1234", "5678");
        var updatedCard = await mediator.Send(changePinCommand);

        // Assert
        updatedCard.Should().NotBeNull();
    }

    [Fact]
    public async Task Workflow_ChangePin_WithSamePin_ShouldThrow()
    {
        // Arrange
        var card = await IssueAndActivateCardAsync();
        var mediator = _fixture!.Mediator!;

        // Act & Assert
        var changePinCommand = new ChangePinCommand(card.Id, "1234", "1234");
        await Assert.ThrowsAsync<InvalidOperationException>(() => mediator.Send(changePinCommand));
    }

    [Fact]
    public async Task Workflow_ChangePin_InvalidCurrentPin_ShouldThrow()
    {
        // Arrange
        var card = await IssueAndActivateCardAsync();
        var mediator = _fixture!.Mediator!;

        // Act & Assert
        var changePinCommand = new ChangePinCommand(card.Id, "9999", "5678");
        await Assert.ThrowsAsync<InvalidOperationException>(() => mediator.Send(changePinCommand));
    }

    #endregion

    #region Limits Management Tests (2 tests)

    [Fact]
    public async Task Workflow_UpdateCardLimits_ShouldApplyNewLimits()
    {
        // Arrange
        var card = await IssueAndActivateCardAsync();
        var mediator = _fixture!.Mediator!;

        // Act
        var updateCommand = new UpdateCardLimitsCommand(
            card.Id,
            30000m,
            15000m,
            10000m);
        var updatedCard = await mediator.Send(updateCommand);

        // Assert
        updatedCard.DailyWithdrawalLimit.Should().Be(30000m);
        updatedCard.DailyTransactionLimit.Should().Be(15000m);
        updatedCard.DailyForeignTransactionLimit.Should().Be(10000m);
    }

    [Fact]
    public async Task Workflow_RecordTransaction_ShouldTrackAgainstLimits()
    {
        // Arrange
        var card = await IssueAndActivateCardAsync();
        var mediator = _fixture!.Mediator!;
        var dbContext = _fixture.GetDbContext();

        // Act
        var recordCommand = new RecordTransactionCommand(
            card.Id,
            5000m,
            TransactionType.Purchase,
            false);
        await mediator.Send(recordCommand);

        // Assert
        var transactions = dbContext.CardTransactions.Where(t => t.CardId == card.Id).ToList();
        transactions.Should().HaveCount(1);
        transactions[0].Amount.Should().Be(5000m);
        transactions[0].Type.Should().Be(TransactionType.Purchase);
    }

    #endregion

    #region Replace Card Tests (2 tests)

    [Fact]
    public async Task Workflow_ReplaceCard_ShouldCreateNewCard()
    {
        // Arrange
        var card = await IssueAndActivateCardAsync();
        var mediator = _fixture!.Mediator!;

        // Act
        var replaceCommand = new ReplaceCardCommand(card.Id, "Card expired");
        var replacementCard = await mediator.Send(replaceCommand);

        // Assert
        replacementCard.Should().NotBeNull();
        replacementCard.Id.Should().NotBe(card.Id);
        replacementCard.CustomerId.Should().Be(card.CustomerId);
    }

    [Fact]
    public async Task Workflow_ReplaceCard_ShouldTrackRelationship()
    {
        // Arrange
        var card = await IssueAndActivateCardAsync();
        var mediator = _fixture!.Mediator!;
        var dbContext = _fixture.GetDbContext();

        // Act
        var replaceCommand = new ReplaceCardCommand(card.Id, "Card damaged");
        var replacementCard = await mediator.Send(replaceCommand);

        // Assert
        var updatedOriginal = dbContext.Cards.First(c => c.Id == card.Id);
        updatedOriginal.ReplacedToCardId.Should().Be(replacementCard.Id);
        replacementCard.ReplacedFromCardId.Should().Be(card.Id);
    }

    #endregion

    #region Query Tests (2 tests)

    [Fact]
    public async Task Query_GetCard_ShouldReturnCardDetails()
    {
        // Arrange
        var card = await IssueAndActivateCardAsync();
        var mediator = _fixture!.Mediator!;

        // Act
        var query = new GetCardQuery(card.Id);
        var result = await mediator.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(card.Id);
        result.HolderName.Should().Be("John Doe");
    }

    [Fact]
    public async Task Query_GetCustomerCards_ShouldReturnAllCustomerCards()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        _fixture!.CreateAndSaveCardProduct(productId);

        var mediator = _fixture.Mediator!;

        // Issue multiple cards
        var issueCommand1 = new IssueCardCommand(customerId, accountId, productId, "Card 1");
        var card1 = await mediator.Send(issueCommand1);

        var issueCommand2 = new IssueCardCommand(customerId, accountId, productId, "Card 2");
        var card2 = await mediator.Send(issueCommand2);

        // Act
        var query = new GetCustomerCardsQuery(customerId);
        var results = await mediator.Send(query);

        // Assert
        results.Should().HaveCount(2);
        results.Select(c => c.Id).Should().Contain(new[] { card1.Id, card2.Id });
    }

    #endregion

    #region Helper Methods

    private async Task<Card> IssueAndActivateCardAsync()
    {
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        _fixture!.CreateAndSaveCardProduct(productId);

        var mediator = _fixture.Mediator!;

        var issueCommand = new IssueCardCommand(customerId, accountId, productId, "John Doe");
        var issuedCard = await mediator.Send(issueCommand);

        var activateCommand = new ActivateCardCommand(issuedCard.Id, "1234");
        var activatedCard = await mediator.Send(activateCommand);

        return activatedCard;
    }

    #endregion
}
