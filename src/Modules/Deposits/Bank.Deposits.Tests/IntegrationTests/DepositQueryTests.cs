namespace Bank.Deposits.Tests.IntegrationTests;

/// <summary>
/// Integration tests for deposit query operations
/// Tests: 8 cases covering deposit retrieval and balance queries
/// </summary>
[Collection("Deposits Integration Tests")]
public class DepositQueryTests
{
    private readonly DepositsTestFixture _fixture;

    public DepositQueryTests(DepositsTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetDepositById_WithValidId_ReturnsDeposit()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var customerId = Guid.NewGuid();
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(customerId, depositType.Id, 1000m, false, null));

        // Act
        var query = new GetDepositByIdQuery(deposit.Id);
        var result = await _fixture.Mediator!.Send(query);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(deposit.Id);
        result.AccountNumber.Should().Be(deposit.AccountNumber);
        result.CurrentBalance.Should().Be(1000m);
    }

    [Fact]
    public async Task GetDepositById_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var query = new GetDepositByIdQuery(invalidId);
        var result = await _fixture.Mediator!.Send(query);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCustomerDeposits_ReturnsAllCustomerDeposits()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var customerId = Guid.NewGuid();

        // Create multiple deposits for same customer
        await _fixture.Mediator!.Send(new OpenDepositCommand(customerId, depositType.Id, 1000m, false, null));
        await _fixture.Mediator!.Send(new OpenDepositCommand(customerId, depositType.Id, 2000m, false, null));

        // Act
        var query = new GetCustomerDepositsQuery(customerId, 1, 10);
        var result = await _fixture.Mediator!.Send(query);

        // Assert
        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task GetCustomerDeposits_WithPagination_RespectsPageSize()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var customerId = Guid.NewGuid();

        // Create 5 deposits
        for (int i = 0; i < 5; i++)
        {
            await _fixture.Mediator!.Send(new OpenDepositCommand(customerId, depositType.Id, (i + 1) * 1000m, false, null));
        }

        // Act
        var query = new GetCustomerDepositsQuery(customerId, 1, 3);
        var result = await _fixture.Mediator!.Send(query);

        // Assert
        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(3);
        result.TotalCount.Should().Be(5);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(3);
    }

    [Fact]
    public async Task GetCustomerDeposits_NoDeposits_ReturnsEmptyList()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Act
        var query = new GetCustomerDepositsQuery(customerId, 1, 10);
        var result = await _fixture.Mediator!.Send(query);

        // Assert
        result.Should().NotBeNull();
        result!.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetDepositBalance_ReturnsCorrectBalance()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 1000m, false, null));

        // Act deposit additional funds
        await _fixture.Mediator!.Send(new DepositFundsCommand(deposit.Id, 500m, "Additional"));

        // Act query balance
        var query = new GetDepositBalanceQuery(deposit.Id);
        var result = await _fixture.Mediator!.Send(query);

        // Assert
        result.Should().NotBeNull();
        result!.CurrentBalance.Should().Be(1500m);
        result.TotalBalance.Should().Be(1500m); // No accrued interest yet
    }

    [Fact]
    public async Task GetDepositBalance_IncludesAccruedInterest()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 1000m, false, null));

        // Simulate accrued interest
        var dbDeposit = await dbContext.Deposits.FirstAsync(d => d.Id == deposit.Id);
        dbDeposit.AccrueInterest(50m);
        await dbContext.SaveChangesAsync();

        // Act
        var query = new GetDepositBalanceQuery(deposit.Id);
        var result = await _fixture.Mediator!.Send(query);

        // Assert
        result.Should().NotBeNull();
        result!.AccruedInterest.Should().Be(50m);
        result.TotalBalance.Should().Be(1050m); // CurrentBalance + AccruedInterest
    }
}
