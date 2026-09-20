namespace Bank.Deposits.Tests;

/// <summary>
/// Unit tests for FixedDeposit command handlers
/// Tests CreateFixedDeposit, ProcessMaturity, EnableAutoRenewal, and ExecuteEarlyWithdrawal commands
/// </summary>
public class FixedDepositCommandHandlerTests
{
    private readonly Guid _customerId = Guid.NewGuid();
    private readonly Guid _linkedAccountId = Guid.NewGuid();
    private readonly Guid _depositProductId = Guid.NewGuid();

    #region CreateFixedDepositCommandHandler Tests

    [Fact]
    public async Task Handle_CreateFixedDepositCommand_WithValidData_ShouldCreateDeposit()
    {
        // Arrange
        var mockFixedDepositRepository = new Mock<IFixedDepositRepository>();
        var mockDepositProductRepository = new Mock<IDepositProductRepository>();
        var mockNumberGenerator = new Mock<IFixedDepositNumberGenerator>();

        var depositProduct = CreateValidDepositProduct();
        mockDepositProductRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(depositProduct);

        var generatedNumber = "FD20260920001234";
        mockNumberGenerator
            .Setup(x => x.GenerateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(generatedNumber);

        var handler = new CreateFixedDepositCommandHandler(
            mockFixedDepositRepository.Object,
            mockDepositProductRepository.Object,
            mockNumberGenerator.Object);

        var command = new CreateFixedDepositCommand(
            _customerId,
            _depositProductId,
            10000m,
            12,
            DateTime.UtcNow,
            false);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.CustomerId.Should().Be(_customerId);
        result.DepositProductId.Should().Be(_depositProductId);
        result.PrincipalAmount.Should().Be(10000m);
        result.Status.Should().Be(FixedDepositStatus.Active);
        result.AutoRenewalEnabled.Should().BeFalse();
        mockFixedDepositRepository.Verify(
            x => x.AddAsync(It.IsAny<FixedDeposit>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_CreateFixedDepositCommand_WithInactiveProduct_ShouldThrowException()
    {
        // Arrange
        var mockFixedDepositRepository = new Mock<IFixedDepositRepository>();
        var mockDepositProductRepository = new Mock<IDepositProductRepository>();
        var mockNumberGenerator = new Mock<IFixedDepositNumberGenerator>();

        var depositProduct = CreateValidDepositProduct();
        depositProduct.Deactivate(); // Make it inactive

        mockDepositProductRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(depositProduct);

        var handler = new CreateFixedDepositCommandHandler(
            mockFixedDepositRepository.Object,
            mockDepositProductRepository.Object,
            mockNumberGenerator.Object);

        var command = new CreateFixedDepositCommand(
            _customerId,
            _depositProductId,
            10000m,
            12,
            DateTime.UtcNow,
            false);

        // Act & Assert
        await handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Handle_CreateFixedDepositCommand_WithAmountOutsideLimits_ShouldThrowException()
    {
        // Arrange
        var mockFixedDepositRepository = new Mock<IFixedDepositRepository>();
        var mockDepositProductRepository = new Mock<IDepositProductRepository>();
        var mockNumberGenerator = new Mock<IFixedDepositNumberGenerator>();

        var depositProduct = CreateValidDepositProduct();
        mockDepositProductRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(depositProduct);

        var handler = new CreateFixedDepositCommandHandler(
            mockFixedDepositRepository.Object,
            mockDepositProductRepository.Object,
            mockNumberGenerator.Object);

        var command = new CreateFixedDepositCommand(
            _customerId,
            _depositProductId,
            50m, // Below minimum
            12,
            DateTime.UtcNow,
            false);

        // Act & Assert
        await handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Handle_CreateFixedDepositCommand_WithTermOutsideLimits_ShouldThrowException()
    {
        // Arrange
        var mockFixedDepositRepository = new Mock<IFixedDepositRepository>();
        var mockDepositProductRepository = new Mock<IDepositProductRepository>();
        var mockNumberGenerator = new Mock<IFixedDepositNumberGenerator>();

        var depositProduct = CreateValidDepositProduct();
        mockDepositProductRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(depositProduct);

        var handler = new CreateFixedDepositCommandHandler(
            mockFixedDepositRepository.Object,
            mockDepositProductRepository.Object,
            mockNumberGenerator.Object);

        var command = new CreateFixedDepositCommand(
            _customerId,
            _depositProductId,
            10000m,
            36, // Above maximum (24 months)
            DateTime.UtcNow,
            false);

        // Act & Assert
        await handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Handle_CreateFixedDepositCommand_WithCustomInterestMethod_ShouldUseCustomMethod()
    {
        // Arrange
        var mockFixedDepositRepository = new Mock<IFixedDepositRepository>();
        var mockDepositProductRepository = new Mock<IDepositProductRepository>();
        var mockNumberGenerator = new Mock<IFixedDepositNumberGenerator>();

        var depositProduct = CreateValidDepositProduct();
        mockDepositProductRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(depositProduct);

        mockNumberGenerator
            .Setup(x => x.GenerateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("FD20260920001234");

        var handler = new CreateFixedDepositCommandHandler(
            mockFixedDepositRepository.Object,
            mockDepositProductRepository.Object,
            mockNumberGenerator.Object);

        var command = new CreateFixedDepositCommand(
            _customerId,
            _depositProductId,
            10000m,
            12,
            DateTime.UtcNow,
            false,
            InterestCalculationMethod.CompoundDaily);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.InterestCalculationMethod.Should().Be(InterestCalculationMethod.CompoundDaily);
    }

    #endregion

    #region ProcessMaturityCommandHandler Tests

    [Fact]
    public async Task Handle_ProcessMaturityCommand_WithPayoutAction_ShouldCloseDeposit()
    {
        // Arrange
        var mockFixedDepositRepository = new Mock<IFixedDepositRepository>();
        var mockDepositProductRepository = new Mock<IDepositProductRepository>();

        var fixedDeposit = CreateActiveFixedDeposit();
        fixedDeposit.MarkAsMatured();

        mockFixedDepositRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixedDeposit);

        var handler = new ProcessMaturityCommandHandler(
            mockFixedDepositRepository.Object,
            mockDepositProductRepository.Object);

        var command = new ProcessMaturityCommand(
            fixedDeposit.Id,
            MaturityAction.Payout);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.Should().Be(FixedDepositStatus.Closed);
        mockFixedDepositRepository.Verify(
            x => x.UpdateAsync(It.IsAny<FixedDeposit>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ProcessMaturityCommand_WithRenewalAction_ShouldCreateRenewedDeposit()
    {
        // Arrange
        var mockFixedDepositRepository = new Mock<IFixedDepositRepository>();
        var mockDepositProductRepository = new Mock<IDepositProductRepository>();

        var fixedDeposit = CreateActiveFixedDeposit();
        fixedDeposit.MarkAsMatured();

        var renewalProduct = CreateValidDepositProduct();

        mockFixedDepositRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixedDeposit);

        mockDepositProductRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(renewalProduct);

        var handler = new ProcessMaturityCommandHandler(
            mockFixedDepositRepository.Object,
            mockDepositProductRepository.Object);

        var command = new ProcessMaturityCommand(
            fixedDeposit.Id,
            MaturityAction.Renewal,
            renewalProduct.Id,
            12);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.Should().Be(FixedDepositStatus.Renewed);
        mockFixedDepositRepository.Verify(
            x => x.AddAsync(It.IsAny<FixedDeposit>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ProcessMaturityCommand_WithReinvestmentAction_ShouldMarkAsMatured()
    {
        // Arrange
        var mockFixedDepositRepository = new Mock<IFixedDepositRepository>();
        var mockDepositProductRepository = new Mock<IDepositProductRepository>();

        var fixedDeposit = CreateActiveFixedDeposit();
        fixedDeposit.MarkAsMatured();

        mockFixedDepositRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixedDeposit);

        var handler = new ProcessMaturityCommandHandler(
            mockFixedDepositRepository.Object,
            mockDepositProductRepository.Object);

        var command = new ProcessMaturityCommand(
            fixedDeposit.Id,
            MaturityAction.Reinvestment);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.Should().Be(FixedDepositStatus.Matured);
    }

    [Fact]
    public async Task Handle_ProcessMaturityCommand_OnNotMaturedDeposit_ShouldThrowException()
    {
        // Arrange
        var mockFixedDepositRepository = new Mock<IFixedDepositRepository>();
        var mockDepositProductRepository = new Mock<IDepositProductRepository>();

        var fixedDeposit = CreateActiveFixedDeposit();

        mockFixedDepositRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixedDeposit);

        var handler = new ProcessMaturityCommandHandler(
            mockFixedDepositRepository.Object,
            mockDepositProductRepository.Object);

        var command = new ProcessMaturityCommand(
            fixedDeposit.Id,
            MaturityAction.Payout);

        // Act & Assert
        await handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>();
    }

    #endregion

    #region EnableAutoRenewalCommandHandler Tests

    [Fact]
    public async Task Handle_EnableAutoRenewalCommand_WithTrueValue_ShouldEnableAutoRenewal()
    {
        // Arrange
        var mockFixedDepositRepository = new Mock<IFixedDepositRepository>();

        var fixedDeposit = CreateActiveFixedDeposit();

        mockFixedDepositRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixedDeposit);

        var handler = new EnableAutoRenewalCommandHandler(mockFixedDepositRepository.Object);

        var command = new EnableAutoRenewalCommand(fixedDeposit.Id, true);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.AutoRenewalEnabled.Should().BeTrue();
        result.RenewalTermDays.Should().Be(fixedDeposit.TermDays);
        mockFixedDepositRepository.Verify(
            x => x.UpdateAsync(It.IsAny<FixedDeposit>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_EnableAutoRenewalCommand_OnNonExistentDeposit_ShouldThrowException()
    {
        // Arrange
        var mockFixedDepositRepository = new Mock<IFixedDepositRepository>();

        mockFixedDepositRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((FixedDeposit?)null);

        var handler = new EnableAutoRenewalCommandHandler(mockFixedDepositRepository.Object);

        var command = new EnableAutoRenewalCommand(Guid.NewGuid(), true);

        // Act & Assert
        await handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>();
    }

    #endregion

    #region ExecuteEarlyWithdrawalCommandHandler Tests

    [Fact]
    public async Task Handle_ExecuteEarlyWithdrawalCommand_WithValidAmount_ShouldProcessWithdrawal()
    {
        // Arrange
        var mockFixedDepositRepository = new Mock<IFixedDepositRepository>();

        var fixedDeposit = CreateActiveFixedDeposit();

        mockFixedDepositRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixedDeposit);

        var handler = new ExecuteEarlyWithdrawalCommandHandler(mockFixedDepositRepository.Object);

        var command = new ExecuteEarlyWithdrawalCommand(fixedDeposit.Id, 5000m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        mockFixedDepositRepository.Verify(
            x => x.UpdateAsync(It.IsAny<FixedDeposit>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ExecuteEarlyWithdrawalCommand_WithAmountExceedingPrincipal_ShouldThrowException()
    {
        // Arrange
        var mockFixedDepositRepository = new Mock<IFixedDepositRepository>();

        var fixedDeposit = CreateActiveFixedDeposit();

        mockFixedDepositRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixedDeposit);

        var handler = new ExecuteEarlyWithdrawalCommandHandler(mockFixedDepositRepository.Object);

        var command = new ExecuteEarlyWithdrawalCommand(fixedDeposit.Id, 15000m);

        // Act & Assert
        await handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Handle_ExecuteEarlyWithdrawalCommand_WithInactiveDeposit_ShouldThrowException()
    {
        // Arrange
        var mockFixedDepositRepository = new Mock<IFixedDepositRepository>();

        var fixedDeposit = CreateActiveFixedDeposit();
        fixedDeposit.Close();

        mockFixedDepositRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixedDeposit);

        var handler = new ExecuteEarlyWithdrawalCommandHandler(mockFixedDepositRepository.Object);

        var command = new ExecuteEarlyWithdrawalCommand(fixedDeposit.Id, 5000m);

        // Act & Assert
        await handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Handle_ExecuteEarlyWithdrawalCommand_WithZeroAmount_ShouldThrowException()
    {
        // Arrange
        var mockFixedDepositRepository = new Mock<IFixedDepositRepository>();

        var fixedDeposit = CreateActiveFixedDeposit();

        mockFixedDepositRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixedDeposit);

        var handler = new ExecuteEarlyWithdrawalCommandHandler(mockFixedDepositRepository.Object);

        var command = new ExecuteEarlyWithdrawalCommand(fixedDeposit.Id, 0m);

        // Act & Assert
        await handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>();
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

    private static DepositProduct CreateValidDepositProduct()
    {
        return DepositProduct.Create(
            "Fixed Deposit - 12 Months",
            "A fixed deposit for 12 months",
            minimumAmount: 1000m,
            maximumAmount: 1000000m,
            minimumTermDays: 90,
            maximumTermDays: 730,
            baseInterestRate: 6.5m,
            interestCalculationMethod: InterestCalculationMethod.Simple,
            compoundingFrequency: InterestFrequency.Monthly,
            defaultPenaltyType: WithdrawalPenaltyType.Percentage,
            defaultPenaltyPercentage: 2m);
    }

    #endregion
}
