namespace Bank.Deposits.Tests;

/// <summary>
/// Unit tests for FixedDeposit query handlers
/// Tests GetFixedDeposit and GetMaturitySchedule queries
/// </summary>
public class FixedDepositQueryHandlerTests
{
    #region GetFixedDepositQueryHandler Tests

    [Fact]
    public async Task Handle_GetFixedDepositQuery_WithValidId_ShouldReturnDto()
    {
        // Arrange
        var mockFixedDepositRepository = new Mock<IFixedDepositRepository>();

        var fixedDeposit = CreateActiveFixedDeposit();

        mockFixedDepositRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixedDeposit);

        var handler = new GetFixedDepositQueryHandler(mockFixedDepositRepository.Object);

        var query = new GetFixedDepositQuery(fixedDeposit.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(fixedDeposit.Id);
        result.DepositNumber.Should().Be(fixedDeposit.DepositNumber);
        result.CustomerId.Should().Be(fixedDeposit.CustomerId);
        result.PrincipalAmount.Should().Be(fixedDeposit.PrincipalAmount);
        result.Status.Should().Be(fixedDeposit.Status);
        result.InterestRate.Should().Be(fixedDeposit.InterestRate);
    }

    [Fact]
    public async Task Handle_GetFixedDepositQuery_WithNonExistentId_ShouldThrowException()
    {
        // Arrange
        var mockFixedDepositRepository = new Mock<IFixedDepositRepository>();

        mockFixedDepositRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((FixedDeposit?)null);

        var handler = new GetFixedDepositQueryHandler(mockFixedDepositRepository.Object);

        var query = new GetFixedDepositQuery(Guid.NewGuid());

        // Act & Assert
        await handler.Invoking(h => h.Handle(query, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Handle_GetFixedDepositQuery_ShouldMapAllProperties()
    {
        // Arrange
        var mockFixedDepositRepository = new Mock<IFixedDepositRepository>();

        var fixedDeposit = CreateActiveFixedDeposit();
        fixedDeposit.RecordAccruedInterest(250m);

        mockFixedDepositRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixedDeposit);

        var handler = new GetFixedDepositQueryHandler(mockFixedDepositRepository.Object);

        var query = new GetFixedDepositQuery(fixedDeposit.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.AccruedInterest.Should().Be(250m);
        result.CompoundingFrequency.Should().Be(fixedDeposit.CompoundingFrequency);
        result.InterestCalculationMethod.Should().Be(fixedDeposit.InterestCalculationMethod);
        result.AutoRenewalEnabled.Should().Be(fixedDeposit.AutoRenewalEnabled);
        result.CreatedAtUtc.Should().Be(fixedDeposit.CreatedAtUtc);
    }

    [Fact]
    public async Task Handle_GetFixedDepositQuery_ShouldConvertTermDaysToMonths()
    {
        // Arrange
        var mockFixedDepositRepository = new Mock<IFixedDepositRepository>();

        var fixedDeposit = FixedDeposit.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            10000m,
            120, // 120 days ≈ 4 months
            6.5m,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.None);

        mockFixedDepositRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixedDeposit);

        var handler = new GetFixedDepositQueryHandler(mockFixedDepositRepository.Object);

        var query = new GetFixedDepositQuery(fixedDeposit.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.TermMonths.Should().Be(4); // 120 / 30
    }

    #endregion

    #region GetMaturityScheduleQueryHandler Tests

    [Fact]
    public async Task Handle_GetMaturityScheduleQuery_WithValidId_ShouldReturnSchedule()
    {
        // Arrange
        var mockFixedDepositRepository = new Mock<IFixedDepositRepository>();
        var mockMaturityProcessingService = new Mock<IMaturityProcessingService>();

        var fixedDeposit = CreateActiveFixedDeposit();

        mockFixedDepositRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixedDeposit);

        var expectedMaturityAmount = 10500m;
        mockMaturityProcessingService
            .Setup(x => x.CalculateMaturityAmountAsync(It.IsAny<FixedDeposit>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMaturityAmount);

        var handler = new GetMaturityScheduleQueryHandler(
            mockFixedDepositRepository.Object,
            mockMaturityProcessingService.Object);

        var query = new GetMaturityScheduleQuery(fixedDeposit.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(fixedDeposit.Id);
        result.DepositNumber.Should().Be(fixedDeposit.DepositNumber);
        result.PrincipalAmount.Should().Be(fixedDeposit.PrincipalAmount);
        result.InterestRate.Should().Be(fixedDeposit.InterestRate);
        result.MaturityAmount.Should().Be(expectedMaturityAmount);
        result.AutoRenewalEnabled.Should().Be(fixedDeposit.AutoRenewalEnabled);
    }

    [Fact]
    public async Task Handle_GetMaturityScheduleQuery_ShouldCalculateRemainingDays()
    {
        // Arrange
        var mockFixedDepositRepository = new Mock<IFixedDepositRepository>();
        var mockMaturityProcessingService = new Mock<IMaturityProcessingService>();

        var fixedDeposit = CreateActiveFixedDeposit(); // Maturity date set to 365 days from now

        mockFixedDepositRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixedDeposit);

        mockMaturityProcessingService
            .Setup(x => x.CalculateMaturityAmountAsync(It.IsAny<FixedDeposit>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(10500m);

        var handler = new GetMaturityScheduleQueryHandler(
            mockFixedDepositRepository.Object,
            mockMaturityProcessingService.Object);

        var query = new GetMaturityScheduleQuery(fixedDeposit.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.RemainingDays.Should().BeGreaterThan(0);
        result.RemainingDays.Should().BeLessThanOrEqualTo(365);
    }

    [Fact]
    public async Task Handle_GetMaturityScheduleQuery_WithNonExistentId_ShouldThrowException()
    {
        // Arrange
        var mockFixedDepositRepository = new Mock<IFixedDepositRepository>();
        var mockMaturityProcessingService = new Mock<IMaturityProcessingService>();

        mockFixedDepositRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((FixedDeposit?)null);

        var handler = new GetMaturityScheduleQueryHandler(
            mockFixedDepositRepository.Object,
            mockMaturityProcessingService.Object);

        var query = new GetMaturityScheduleQuery(Guid.NewGuid());

        // Act & Assert
        await handler.Invoking(h => h.Handle(query, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Handle_GetMaturityScheduleQuery_ShouldIncludeAccruedInterest()
    {
        // Arrange
        var mockFixedDepositRepository = new Mock<IFixedDepositRepository>();
        var mockMaturityProcessingService = new Mock<IMaturityProcessingService>();

        var fixedDeposit = CreateActiveFixedDeposit();
        fixedDeposit.RecordAccruedInterest(250m);

        mockFixedDepositRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixedDeposit);

        mockMaturityProcessingService
            .Setup(x => x.CalculateMaturityAmountAsync(It.IsAny<FixedDeposit>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(10750m);

        var handler = new GetMaturityScheduleQueryHandler(
            mockFixedDepositRepository.Object,
            mockMaturityProcessingService.Object);

        var query = new GetMaturityScheduleQuery(fixedDeposit.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.AccruedInterest.Should().Be(250m);
    }

    [Fact]
    public async Task Handle_GetMaturityScheduleQuery_ShouldIncludeDepositStatus()
    {
        // Arrange
        var mockFixedDepositRepository = new Mock<IFixedDepositRepository>();
        var mockMaturityProcessingService = new Mock<IMaturityProcessingService>();

        var fixedDeposit = CreateActiveFixedDeposit();

        mockFixedDepositRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixedDeposit);

        mockMaturityProcessingService
            .Setup(x => x.CalculateMaturityAmountAsync(It.IsAny<FixedDeposit>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(10500m);

        var handler = new GetMaturityScheduleQueryHandler(
            mockFixedDepositRepository.Object,
            mockMaturityProcessingService.Object);

        var query = new GetMaturityScheduleQuery(fixedDeposit.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Status.Should().Be(FixedDepositStatus.Active);
    }

    [Fact]
    public async Task Handle_GetMaturityScheduleQuery_ShouldConvertTermDaysToMonths()
    {
        // Arrange
        var mockFixedDepositRepository = new Mock<IFixedDepositRepository>();
        var mockMaturityProcessingService = new Mock<IMaturityProcessingService>();

        var fixedDeposit = FixedDeposit.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            10000m,
            180, // 180 days ≈ 6 months
            6.5m,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.None);

        mockFixedDepositRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixedDeposit);

        mockMaturityProcessingService
            .Setup(x => x.CalculateMaturityAmountAsync(It.IsAny<FixedDeposit>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(10325m);

        var handler = new GetMaturityScheduleQueryHandler(
            mockFixedDepositRepository.Object,
            mockMaturityProcessingService.Object);

        var query = new GetMaturityScheduleQuery(fixedDeposit.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.TermMonths.Should().Be(6); // 180 / 30
    }

    [Fact]
    public async Task Handle_GetMaturityScheduleQuery_ShouldCallMaturityProcessingService()
    {
        // Arrange
        var mockFixedDepositRepository = new Mock<IFixedDepositRepository>();
        var mockMaturityProcessingService = new Mock<IMaturityProcessingService>();

        var fixedDeposit = CreateActiveFixedDeposit();

        mockFixedDepositRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixedDeposit);

        mockMaturityProcessingService
            .Setup(x => x.CalculateMaturityAmountAsync(It.IsAny<FixedDeposit>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(10500m);

        var handler = new GetMaturityScheduleQueryHandler(
            mockFixedDepositRepository.Object,
            mockMaturityProcessingService.Object);

        var query = new GetMaturityScheduleQuery(fixedDeposit.Id);

        // Act
        await handler.Handle(query, CancellationToken.None);

        // Assert
        mockMaturityProcessingService.Verify(
            x => x.CalculateMaturityAmountAsync(It.IsAny<FixedDeposit>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region Helper Methods

    private static FixedDeposit CreateActiveFixedDeposit()
    {
        return FixedDeposit.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            10000m,
            365,
            6.5m,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.Percentage,
            penaltyPercentage: 2m);
    }

    #endregion
}
