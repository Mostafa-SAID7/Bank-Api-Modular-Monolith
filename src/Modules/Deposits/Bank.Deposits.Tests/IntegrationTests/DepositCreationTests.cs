namespace Bank.Deposits.Tests.IntegrationTests;

/// <summary>
/// Integration tests for deposit creation operations
/// Tests: 15 cases covering various deposit types and validation scenarios
/// </summary>
[Collection("Deposits Integration Tests")]
public class DepositCreationTests
{
    private readonly DepositsTestFixture _fixture;

    public DepositCreationTests(DepositsTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task OpenDeposit_WithValidSavingsAccountData_CreatesDepositSuccessfully()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var customerId = Guid.NewGuid();
        var command = new OpenDepositCommand(customerId, depositType.Id, 500m, false, null);

        // Act
        var result = await _fixture.Mediator!.Send(command);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.CustomerId.Should().Be(customerId);
        result.CurrentBalance.Should().Be(500m);
        result.Status.Should().Be(DepositStatus.Active);
        result.IsFixedDeposit.Should().BeFalse();
        result.AccountNumber.Should().StartWith("DEP");
    }

    [Fact]
    public async Task OpenDeposit_WithValidFixedDepositData_CreatesFixedDepositSuccessfully()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Fixed Deposit - 12 Months");
        var customerId = Guid.NewGuid();
        var command = new OpenDepositCommand(customerId, depositType.Id, 10000m, true, 12);

        // Act
        var result = await _fixture.Mediator!.Send(command);

        // Assert
        result.Should().NotBeNull();
        result.IsFixedDeposit.Should().BeTrue();
        result.TermMonths.Should().Be(12);
        result.MaturityDateUtc.Should().BeCloseTo(DateTime.UtcNow.AddMonths(12), TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task OpenDeposit_WithBelowMinimumBalance_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Fixed Deposit - 12 Months");
        var customerId = Guid.NewGuid();
        var command = new OpenDepositCommand(customerId, depositType.Id, 500m, true, 12);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _fixture.Mediator!.Send(command));
    }

    [Fact]
    public async Task OpenDeposit_WithAboveMaximumBalance_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Fixed Deposit - 12 Months");
        var customerId = Guid.NewGuid();
        var command = new OpenDepositCommand(customerId, depositType.Id, 2000000m, true, 12);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _fixture.Mediator!.Send(command));
    }

    [Fact]
    public async Task OpenDeposit_WithNegativeInitialDeposit_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var customerId = Guid.NewGuid();
        var command = new OpenDepositCommand(customerId, depositType.Id, -100m, false, null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _fixture.Mediator!.Send(command));
    }

    [Fact]
    public async Task OpenDeposit_WithZeroInitialDeposit_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var customerId = Guid.NewGuid();
        var command = new OpenDepositCommand(customerId, depositType.Id, 0m, false, null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _fixture.Mediator!.Send(command));
    }

    [Fact]
    public async Task OpenDeposit_GeneratesUniqueAccountNumbers()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var customerId1 = Guid.NewGuid();
        var customerId2 = Guid.NewGuid();

        // Act
        var deposit1 = await _fixture.Mediator!.Send(new OpenDepositCommand(customerId1, depositType.Id, 500m, false, null));
        var deposit2 = await _fixture.Mediator!.Send(new OpenDepositCommand(customerId2, depositType.Id, 500m, false, null));

        // Assert
        deposit1.AccountNumber.Should().NotBe(deposit2.AccountNumber);
    }

    [Fact]
    public async Task OpenDeposit_WithInactiveDepositType_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var inactiveType = new DepositType
        {
            Id = Guid.NewGuid(),
            Name = "Inactive Type",
            MinimumBalance = 100m,
            IsActive = false,
            CreatedAtUtc = DateTime.UtcNow
        };
        await dbContext.DepositTypes.AddAsync(inactiveType);
        await dbContext.SaveChangesAsync();

        var command = new OpenDepositCommand(Guid.NewGuid(), inactiveType.Id, 500m, false, null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _fixture.Mediator!.Send(command));
    }

    [Fact]
    public async Task OpenDeposit_WithNonExistentDepositType_ThrowsInvalidOperationException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var command = new OpenDepositCommand(customerId, Guid.NewGuid(), 500m, false, null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _fixture.Mediator!.Send(command));
    }

    [Fact]
    public async Task OpenDeposit_FixedDepositMissingTermMonths_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Fixed Deposit - 12 Months");
        var customerId = Guid.NewGuid();
        var command = new OpenDepositCommand(customerId, depositType.Id, 5000m, true, null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _fixture.Mediator!.Send(command));
    }

    [Fact]
    public async Task OpenDeposit_CreatedDepositIsPersisted()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var customerId = Guid.NewGuid();
        var command = new OpenDepositCommand(customerId, depositType.Id, 750m, false, null);

        // Act
        var result = await _fixture.Mediator!.Send(command);

        // Assert - Verify deposit is in database
        var persistedDeposit = await dbContext.Deposits.FirstOrDefaultAsync(d => d.Id == result.Id);
        persistedDeposit.Should().NotBeNull();
        persistedDeposit!.AccountNumber.Should().Be(result.AccountNumber);
        persistedDeposit.CurrentBalance.Should().Be(750m);
    }
}
