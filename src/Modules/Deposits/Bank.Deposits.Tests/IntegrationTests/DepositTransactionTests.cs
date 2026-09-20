namespace Bank.Deposits.Tests.IntegrationTests;

/// <summary>
/// Integration tests for deposit transactions (deposit/withdrawal)
/// Tests: 18 cases covering deposit and withdrawal operations with various constraints
/// </summary>
[Collection("Deposits Integration Tests")]
public class DepositTransactionTests
{
    private readonly DepositsTestFixture _fixture;

    public DepositTransactionTests(DepositsTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DepositFunds_WithValidAmount_IncreasesBalance()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 1000m, false, null));

        // Act
        var result = await _fixture.Mediator!.Send(new DepositFundsCommand(deposit.Id, 500m, "Additional deposit"));

        // Assert
        result.CurrentBalance.Should().Be(1500m);
    }

    [Fact]
    public async Task DepositFunds_WithMultipleDeposits_BalanceAccumulates()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 1000m, false, null));

        // Act
        await _fixture.Mediator!.Send(new DepositFundsCommand(deposit.Id, 200m, "Deposit 1"));
        await _fixture.Mediator!.Send(new DepositFundsCommand(deposit.Id, 300m, "Deposit 2"));
        var result = await _fixture.Mediator!.Send(new DepositFundsCommand(deposit.Id, 100m, "Deposit 3"));

        // Assert
        result.CurrentBalance.Should().Be(1600m);
    }

    [Fact]
    public async Task DepositFunds_IntoFrozenAccount_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 1000m, false, null));
        await _fixture.Mediator!.Send(new FreezeDepositCommand(deposit.Id, "Testing freeze"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _fixture.Mediator!.Send(new DepositFundsCommand(deposit.Id, 500m, "Should fail")));
    }

    [Fact]
    public async Task DepositFunds_IntoClosedAccount_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 1000m, false, null));
        await _fixture.Mediator!.Send(new CloseDepositCommand(deposit.Id, "Testing closure"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _fixture.Mediator!.Send(new DepositFundsCommand(deposit.Id, 500m, "Should fail")));
    }

    [Fact]
    public async Task WithdrawFunds_WithValidAmount_DecreasesBalance()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 1000m, false, null));

        // Act
        var result = await _fixture.Mediator!.Send(new WithdrawFundsCommand(deposit.Id, 300m, "Withdrawal for bills"));

        // Assert
        result.CurrentBalance.Should().Be(700m);
    }

    [Fact]
    public async Task WithdrawFunds_MoreThanBalance_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 1000m, false, null));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _fixture.Mediator!.Send(new WithdrawFundsCommand(deposit.Id, 2000m, "Over limit")));
    }

    [Fact]
    public async Task WithdrawFunds_ExactBalance_ReducesToZero()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 1000m, false, null));

        // Act
        var result = await _fixture.Mediator!.Send(new WithdrawFundsCommand(deposit.Id, 1000m, "Complete withdrawal"));

        // Assert
        result.CurrentBalance.Should().Be(0m);
    }

    [Fact]
    public async Task WithdrawFunds_FromFrozenAccount_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 1000m, false, null));
        await _fixture.Mediator!.Send(new FreezeDepositCommand(deposit.Id, "Testing freeze"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _fixture.Mediator!.Send(new WithdrawFundsCommand(deposit.Id, 100m, "Should fail")));
    }

    [Fact]
    public async Task WithdrawFunds_FromClosedAccount_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 1000m, false, null));
        await _fixture.Mediator!.Send(new CloseDepositCommand(deposit.Id, "Testing closure"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _fixture.Mediator!.Send(new WithdrawFundsCommand(deposit.Id, 100m, "Should fail")));
    }

    [Fact]
    public async Task WithdrawFunds_FromFixedDepositBeforeMaturity_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Fixed Deposit - 12 Months");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 5000m, true, 12));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _fixture.Mediator!.Send(new WithdrawFundsCommand(deposit.Id, 1000m, "Premature withdrawal")));
    }

    [Fact]
    public async Task WithdrawFunds_NegativeAmount_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 1000m, false, null));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _fixture.Mediator!.Send(new WithdrawFundsCommand(deposit.Id, -100m, "Negative")));
    }

    [Fact]
    public async Task WithdrawFunds_ZeroAmount_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 1000m, false, null));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _fixture.Mediator!.Send(new WithdrawFundsCommand(deposit.Id, 0m, "Zero")));
    }

    [Fact]
    public async Task WithdrawFunds_IsPersisted()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 1000m, false, null));

        // Act
        await _fixture.Mediator!.Send(new WithdrawFundsCommand(deposit.Id, 250m, "Test withdrawal"));

        // Assert - Verify in database
        var persistedDeposit = await dbContext.Deposits.FirstAsync(d => d.Id == deposit.Id);
        persistedDeposit.CurrentBalance.Should().Be(750m);
    }
}
