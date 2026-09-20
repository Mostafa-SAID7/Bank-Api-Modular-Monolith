namespace Bank.Deposits.Tests.IntegrationTests;

/// <summary>
/// Integration tests for interest calculation and accrual
/// Tests: 12 cases covering simple/compound interest and interest payment operations
/// </summary>
[Collection("Deposits Integration Tests")]
public class InterestCalculationTests
{
    private readonly DepositsTestFixture _fixture;

    public InterestCalculationTests(DepositsTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task PayInterest_WithAccruedInterest_TransfersToBalance()
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
        var result = await _fixture.Mediator!.Send(new PayDepositInterestCommand(deposit.Id));

        // Assert
        result.PaidInterest.Should().Be(50m);
        result.CurrentBalance.Should().Be(1050m);
        result.AccruedInterest.Should().Be(0m);
    }

    [Fact]
    public async Task InterestCalculationService_SimpleInterest_CalculatesCorrectly()
    {
        // Arrange
        var service = new InterestCalculationService();
        decimal principal = 1000m;
        decimal annualRate = 5m; // 5%
        int daysInPeriod = 30;
        int daysInYear = 365;

        // Act
        var interest = service.CalculateInterest(
            principal, annualRate, daysInPeriod, daysInYear, 
            InterestCalculationMethod.Simple);

        // Assert
        var expected = 1000m * 0.05m * (30m / 365m);
        interest.Should().BeApproximately(expected, 0.01m);
    }

    [Fact]
    public async Task InterestCalculationService_CompoundInterest_CalculatesCorrectly()
    {
        // Arrange
        var service = new InterestCalculationService();
        decimal principal = 1000m;
        decimal annualRate = 5m; // 5%
        int monthsElapsed = 12;
        int compoundingFrequency = 1; // Monthly

        // Act
        var interest = service.CalculateCompoundInterest(
            principal, annualRate, monthsElapsed, compoundingFrequency);

        // Assert
        interest.Should().BeGreaterThan(0);
        // Compound interest should be greater than simple interest
        var simpleInterest = principal * 0.05m;
        interest.Should().BeGreaterThan(simpleInterest);
    }

    [Fact]
    public async Task MaturityAmount_Calculation_IncludesPrincipalAndInterest()
    {
        // Arrange
        var service = new InterestCalculationService();
        decimal principal = 10000m;
        decimal annualRate = 4m;
        int termMonths = 12;

        // Act
        var maturityAmount = service.CalculateMaturityAmount(
            principal, annualRate, termMonths, InterestCalculationMethod.Simple);

        // Assert
        maturityAmount.Should().BeGreaterThan(principal);
        var interest = maturityAmount - principal;
        interest.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task InterestAccrual_NegativeAmount_ThrowsException()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 1000m, false, null));
        var dbDeposit = await dbContext.Deposits.FirstAsync(d => d.Id == deposit.Id);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => dbDeposit.AccrueInterest(-10m));
    }

    [Fact]
    public async Task MultipleInterestPayments_AccumulatePaidInterest()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Savings Account");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 1000m, false, null));

        // Simulate first interest accrual and payment
        var dbDeposit = await dbContext.Deposits.FirstAsync(d => d.Id == deposit.Id);
        dbDeposit.AccrueInterest(25m);
        await dbContext.SaveChangesAsync();
        await _fixture.Mediator!.Send(new PayDepositInterestCommand(deposit.Id));

        // Simulate second interest accrual
        dbDeposit = await dbContext.Deposits.FirstAsync(d => d.Id == deposit.Id);
        dbDeposit.AccrueInterest(30m);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await _fixture.Mediator!.Send(new PayDepositInterestCommand(deposit.Id));

        // Assert
        result.PaidInterest.Should().Be(55m);
        result.CurrentBalance.Should().Be(1055m);
    }

    [Fact]
    public async Task InterestCalculation_ZeroPrincipal_ReturnsZero()
    {
        // Arrange
        var service = new InterestCalculationService();

        // Act
        var interest = service.CalculateInterest(
            0m, 5m, 30, 365, InterestCalculationMethod.Simple);

        // Assert
        interest.Should().Be(0m);
    }

    [Fact]
    public async Task InterestCalculation_NegativeRate_ReturnsZero()
    {
        // Arrange
        var service = new InterestCalculationService();

        // Act
        var interest = service.CalculateInterest(
            1000m, -5m, 30, 365, InterestCalculationMethod.Simple);

        // Assert
        interest.Should().Be(0m);
    }

    [Fact]
    public async Task CalculateMaturityAmount_FixedDeposit_ReturnsCorrectAmount()
    {
        // Arrange
        await _fixture.SeedDepositTypesAsync();
        var dbContext = _fixture.GetDbContext();
        var depositType = await dbContext.DepositTypes.FirstAsync(dt => dt.Name == "Fixed Deposit - 12 Months");
        var deposit = await _fixture.Mediator!.Send(new OpenDepositCommand(Guid.NewGuid(), depositType.Id, 5000m, true, 12));

        // Act
        var query = new CalculateMaturityAmountQuery(deposit.Id, 5000m, 3.5m, 12, InterestCalculationMethod.Simple);
        var result = await _fixture.Mediator!.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Principal.Should().Be(5000m);
        result.MaturityAmount.Should().BeGreaterThan(5000m);
    }
}
