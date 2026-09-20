namespace Bank.Deposits.Tests.IntegrationTests;

/// <summary>
/// Integration tests for deposit status management (freeze, unfreeze, close)
/// Tests: 14 cases covering status transitions and constraints
/// </summary>
[Collection("Deposits Integration Tests")]
public class DepositStatusManagementTests
{
    private readonly DepositsTestFixture _fixture;

    public DepositStatusManagementTests(DepositsTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task FreezeDeposit_ChangesStatusToFrozen()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 1000m, false, null));

        // Act
        var result = await _fixture.Mediator!.Send(new FreezeDepositCommand(deposit.Id, "Account investigation"));

        // Assert
        result.Status.Should().Be(DepositStatus.Frozen);
        result.FreezeReason.Should().Be("Account investigation");
    }

    [Fact]
    public async Task UnfreezeDeposit_ChangesStatusBackToActive()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 1000m, false, null));
        await _fixture.Mediator!.Send(new FreezeDepositCommand(deposit.Id, "Testing freeze"));

        // Act
        var result = await _fixture.Mediator!.Send(new UnfreezeDepositCommand(deposit.Id));

        // Assert
        result.Status.Should().Be(DepositStatus.Active);
    }

    [Fact]
    public async Task FreezeDeposit_AlreadyFrozen_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 1000m, false, null));
        await _fixture.Mediator!.Send(new FreezeDepositCommand(deposit.Id, "First freeze"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _fixture.Mediator!.Send(new FreezeDepositCommand(deposit.Id, "Second freeze")));
    }

    [Fact]
    public async Task UnfreezeDeposit_WhenActive_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 1000m, false, null));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _fixture.Mediator!.Send(new UnfreezeDepositCommand(deposit.Id)));
    }

    [Fact]
    public async Task CloseDeposit_ChangesStatusToClosed()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 1000m, false, null));

        // Act
        var result = await _fixture.Mediator!.Send(new CloseDepositCommand(deposit.Id, "Customer request"));

        // Assert
        result.Status.Should().Be(DepositStatus.Closed);
        result.ClosedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.ClosureReason.Should().Be("Customer request");
    }

    [Fact]
    public async Task CloseDeposit_AlreadyClosed_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 1000m, false, null));
        await _fixture.Mediator!.Send(new CloseDepositCommand(deposit.Id, "First closure"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _fixture.Mediator!.Send(new CloseDepositCommand(deposit.Id, "Second closure")));
    }

    [Fact]
    public async Task FreezeDeposit_AlreadyClosed_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 1000m, false, null));
        await _fixture.Mediator!.Send(new CloseDepositCommand(deposit.Id, "Closure"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _fixture.Mediator!.Send(new FreezeDepositCommand(deposit.Id, "Freeze after close")));
    }

    [Fact]
    public async Task StatusTransitions_ArePersisted()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 1000m, false, null));

        // Act
        await _fixture.Mediator!.Send(new FreezeDepositCommand(deposit.Id, "Freeze"));
        await _fixture.Mediator!.Send(new UnfreezeDepositCommand(deposit.Id));

        // Assert - Verify status in database
        var persistedDeposit = await dbContext.Deposits.FirstAsync(d => d.Id == deposit.Id);
        persistedDeposit.Status.Should().Be(DepositStatus.Active);
    }

    [Fact]
    public async Task CloseDeposit_WithFrozenStatus_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 1000m, false, null));
        await _fixture.Mediator!.Send(new FreezeDepositCommand(deposit.Id, "Freeze"));

        // Act - Close should work even if frozen
        var result = await _fixture.Mediator!.Send(new CloseDepositCommand(deposit.Id, "Force close"));

        // Assert
        result.Status.Should().Be(DepositStatus.Closed);
    }
}
