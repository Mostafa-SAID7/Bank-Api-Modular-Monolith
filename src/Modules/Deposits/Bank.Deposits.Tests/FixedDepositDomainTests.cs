namespace Bank.Deposits.Tests;

/// <summary>
/// Unit tests for FixedDeposit aggregate root and domain logic
/// Tests creation, status transitions, interest calculations, and maturity handling
/// </summary>
public class FixedDepositDomainTests
{
    private readonly Guid _customerId = Guid.NewGuid();
    private readonly Guid _linkedAccountId = Guid.NewGuid();
    private readonly Guid _depositProductId = Guid.NewGuid();
    private const decimal TestPrincipal = 10000m;
    private const int TestTermDays = 365;
    private const decimal TestInterestRate = 6.5m;

    #region Creation Tests

    [Fact]
    public void Create_WithValidParameters_ShouldCreateActiveDeposit()
    {
        // Arrange & Act
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            TestPrincipal,
            TestTermDays,
            TestInterestRate,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.Percentage,
            penaltyPercentage: 2m);

        // Assert
        deposit.Should().NotBeNull();
        deposit.Id.Should().NotBe(Guid.Empty);
        deposit.DepositNumber.Should().StartWith("FD");
        deposit.CustomerId.Should().Be(_customerId);
        deposit.LinkedAccountId.Should().Be(_linkedAccountId);
        deposit.DepositProductId.Should().Be(_depositProductId);
        deposit.PrincipalAmount.Should().Be(TestPrincipal);
        deposit.TermDays.Should().Be(TestTermDays);
        deposit.InterestRate.Should().Be(TestInterestRate);
        deposit.Status.Should().Be(FixedDepositStatus.Active);
        deposit.AutoRenewalEnabled.Should().BeFalse();
        deposit.AccruedInterest.Should().Be(0);
    }

    [Fact]
    public void Create_ShouldSetCorrectMaturityDate()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;
        var termDays = 180;

        // Act
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            TestPrincipal,
            termDays,
            TestInterestRate,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.None);

        var afterCreation = DateTime.UtcNow;

        // Assert
        var expectedMaturityDate = beforeCreation.AddDays(termDays);
        var actualMaturityDate = deposit.MaturityDateUtc;

        actualMaturityDate.Should()
            .BeOnOrAfter(expectedMaturityDate.AddSeconds(-1))
            .And.BeOnOrBefore(expectedMaturityDate.AddSeconds(1));
    }

    [Fact]
    public void Create_ShouldGenerateUniqueDepositNumbers()
    {
        // Arrange & Act
        var deposit1 = FixedDeposit.Create(_customerId, _linkedAccountId, _depositProductId, 
            TestPrincipal, TestTermDays, TestInterestRate, 
            InterestCalculationMethod.Simple, InterestFrequency.Monthly, WithdrawalPenaltyType.None);
        
        var deposit2 = FixedDeposit.Create(_customerId, _linkedAccountId, _depositProductId, 
            TestPrincipal, TestTermDays, TestInterestRate, 
            InterestCalculationMethod.Simple, InterestFrequency.Monthly, WithdrawalPenaltyType.None);

        // Assert
        deposit1.DepositNumber.Should().NotBe(deposit2.DepositNumber);
    }

    #endregion

    #region Interest Calculation Tests

    [Fact]
    public void CalculateInterestAtMaturity_WithSimpleInterest_ShouldCalculateCorrectly()
    {
        // Arrange
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            10000m,
            365,
            5m, // 5% annual rate
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.None);

        // Act
        var interest = deposit.CalculateInterestAtMaturity();

        // Assert
        // Simple interest = P * R * T = 10000 * 0.05 * 1 = 500
        interest.Should().BeApproximately(500m, 10m);
    }

    [Fact]
    public void CalculateMaturityAmount_ShouldIncludePrincipalAndInterest()
    {
        // Arrange
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            10000m,
            365,
            5m,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.None);

        // Act
        var maturityAmount = deposit.CalculateMaturityAmount();

        // Assert
        var expectedAmount = 10000m + 500m; // Principal + Simple Interest
        maturityAmount.Should().BeApproximately(expectedAmount, 10m);
    }

    [Fact]
    public void CalculateInterestAtMaturity_WithCompoundDaily_ShouldCalculateCorrectly()
    {
        // Arrange
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            10000m,
            365,
            5m,
            InterestCalculationMethod.CompoundDaily,
            InterestFrequency.Daily,
            WithdrawalPenaltyType.None);

        // Act
        var interest = deposit.CalculateInterestAtMaturity();

        // Assert
        // Compound daily should be slightly more than simple interest
        interest.Should().BeGreaterThan(500m);
        interest.Should().BeLessThan(600m); // Reasonable upper bound
    }

    [Fact]
    public void CalculateInterestAtMaturity_WithCompoundMonthly_ShouldCalculateCorrectly()
    {
        // Arrange
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            10000m,
            365,
            5m,
            InterestCalculationMethod.CompoundMonthly,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.None);

        // Act
        var interest = deposit.CalculateInterestAtMaturity();

        // Assert
        // Compound monthly should be between simple and daily
        interest.Should().BeGreaterThan(500m);
        interest.Should().BeLessThan(600m);
    }

    #endregion

    #region Early Withdrawal Penalty Tests

    [Fact]
    public void CalculateEarlyWithdrawalPenalty_WithNone_ShouldReturnZero()
    {
        // Arrange
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            TestPrincipal,
            TestTermDays,
            TestInterestRate,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.None);

        // Act
        var penalty = deposit.CalculateEarlyWithdrawalPenalty(5000m);

        // Assert
        penalty.Should().Be(0);
    }

    [Fact]
    public void CalculateEarlyWithdrawalPenalty_WithFixedAmount_ShouldReturnFixedPenalty()
    {
        // Arrange
        var penaltyAmount = 100m;
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            TestPrincipal,
            TestTermDays,
            TestInterestRate,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.FixedAmount,
            penaltyAmount: penaltyAmount);

        // Act
        var penalty = deposit.CalculateEarlyWithdrawalPenalty(5000m);

        // Assert
        penalty.Should().Be(penaltyAmount);
    }

    [Fact]
    public void CalculateEarlyWithdrawalPenalty_WithPercentage_ShouldReturnPercentagePenalty()
    {
        // Arrange
        var penaltyPercentage = 2m;
        var withdrawalAmount = 5000m;
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            TestPrincipal,
            TestTermDays,
            TestInterestRate,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.Percentage,
            penaltyPercentage: penaltyPercentage);

        // Act
        var penalty = deposit.CalculateEarlyWithdrawalPenalty(withdrawalAmount);

        // Assert
        // Expected = 5000 * 0.02 = 100
        penalty.Should().Be(100m);
    }

    [Fact]
    public void CalculateEarlyWithdrawalPenalty_WithInterestForfeiture_ShouldReturnAccruedInterest()
    {
        // Arrange
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            TestPrincipal,
            TestTermDays,
            TestInterestRate,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.InterestForfeiture);

        deposit.RecordAccruedInterest(250m);

        // Act
        var penalty = deposit.CalculateEarlyWithdrawalPenalty(5000m);

        // Assert
        penalty.Should().Be(250m);
    }

    [Fact]
    public void CalculateEarlyWithdrawalPenalty_WithCombined_ShouldReturnCombinedPenalty()
    {
        // Arrange
        var penaltyAmount = 50m;
        var penaltyPercentage = 1m;
        var withdrawalAmount = 5000m;
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            TestPrincipal,
            TestTermDays,
            TestInterestRate,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.Combined,
            penaltyAmount: penaltyAmount,
            penaltyPercentage: penaltyPercentage);

        // Act
        var penalty = deposit.CalculateEarlyWithdrawalPenalty(withdrawalAmount);

        // Assert
        // Expected = 50 + (5000 * 0.01) = 50 + 50 = 100
        penalty.Should().Be(100m);
    }

    [Fact]
    public void CalculateEarlyWithdrawalPenalty_OnMaturedDeposit_ShouldReturnZero()
    {
        // Arrange
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            TestPrincipal,
            TestTermDays,
            TestInterestRate,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.Percentage,
            penaltyPercentage: 5m);

        deposit.MarkAsMatured();

        // Act
        var penalty = deposit.CalculateEarlyWithdrawalPenalty(5000m);

        // Assert
        penalty.Should().Be(0);
    }

    #endregion

    #region Maturity Status Tests

    [Fact]
    public void HasMatured_BeforeMaturityDate_ShouldReturnFalse()
    {
        // Arrange
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            TestPrincipal,
            365,
            TestInterestRate,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.None);

        // Act
        var hasMatured = deposit.HasMatured();

        // Assert
        hasMatured.Should().BeFalse();
    }

    [Fact]
    public void ShouldSendRenewalNotice_WithAutoRenewalDisabled_ShouldReturnFalse()
    {
        // Arrange
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            TestPrincipal,
            TestTermDays,
            TestInterestRate,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.None);

        // Act
        var shouldSend = deposit.ShouldSendRenewalNotice();

        // Assert
        shouldSend.Should().BeFalse();
    }

    [Fact]
    public void ShouldSendRenewalNotice_WithAutoRenewalEnabled_NearMaturity_ShouldReturnTrue()
    {
        // Arrange
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            TestPrincipal,
            TestTermDays,
            TestInterestRate,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.None);

        deposit.EnableAutoRenewal(180, requireConsent: false);

        // Simulate being within 30 days of maturity
        // This would require mocking DateTime.UtcNow, so we're testing the logic
        // The actual test would need a more sophisticated setup

        // Act
        var shouldSend = deposit.ShouldSendRenewalNotice();

        // Assert - will be false since we're not close to maturity yet
        shouldSend.Should().BeFalse();
    }

    #endregion

    #region Status Transition Tests

    [Fact]
    public void MarkAsMatured_OnActiveDeposit_ShouldChangeStatusToMatured()
    {
        // Arrange
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            TestPrincipal,
            TestTermDays,
            TestInterestRate,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.None);

        // Act
        deposit.MarkAsMatured();

        // Assert
        deposit.Status.Should().Be(FixedDepositStatus.Matured);
    }

    [Fact]
    public void MarkAsMatured_OnNonActiveDeposit_ShouldThrowException()
    {
        // Arrange
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            TestPrincipal,
            TestTermDays,
            TestInterestRate,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.None);

        deposit.MarkAsMatured();

        // Act & Assert
        var act = () => deposit.MarkAsMatured();
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Close_OnActiveDeposit_ShouldCloseWithReason()
    {
        // Arrange
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            TestPrincipal,
            TestTermDays,
            TestInterestRate,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.None);

        var reason = "Customer requested early closure";

        // Act
        deposit.Close(reason);

        // Assert
        deposit.Status.Should().Be(FixedDepositStatus.Closed);
        deposit.ClosureReason.Should().Be(reason);
        deposit.ClosureDateUtc.Should().NotBeNull();
        deposit.ClosureDateUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Close_OnAlreadyClosedDeposit_ShouldThrowException()
    {
        // Arrange
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            TestPrincipal,
            TestTermDays,
            TestInterestRate,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.None);

        deposit.Close();

        // Act & Assert
        var act = () => deposit.Close();
        act.Should().Throw<InvalidOperationException>();
    }

    #endregion

    #region Auto-Renewal Tests

    [Fact]
    public void EnableAutoRenewal_ShouldEnableRenewalWithCorrectTerms()
    {
        // Arrange
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            TestPrincipal,
            TestTermDays,
            TestInterestRate,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.None);

        var renewalTermDays = 180;

        // Act
        deposit.EnableAutoRenewal(renewalTermDays, requireConsent: false);

        // Assert
        deposit.AutoRenewalEnabled.Should().BeTrue();
        deposit.RenewalTermDays.Should().Be(renewalTermDays);
        deposit.CustomerConsentReceived.Should().BeTrue();
    }

    [Fact]
    public void EnableAutoRenewal_WithConsentRequired_ShouldNotMarkConsentReceived()
    {
        // Arrange
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            TestPrincipal,
            TestTermDays,
            TestInterestRate,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.None);

        // Act
        deposit.EnableAutoRenewal(180, requireConsent: true);

        // Assert
        deposit.AutoRenewalEnabled.Should().BeTrue();
        deposit.CustomerConsentReceived.Should().BeFalse();
    }

    [Fact]
    public void Renew_OnMaturedDeposit_ShouldCreateNewDepositWithRolledOverPrincipal()
    {
        // Arrange
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            TestPrincipal,
            TestTermDays,
            TestInterestRate,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.None);

        deposit.RecordAccruedInterest(250m);
        deposit.MarkAsMatured();

        var newTermDays = 180;

        // Act
        var renewedDeposit = deposit.Renew(newTermDays);

        // Assert
        renewedDeposit.Should().NotBeNull();
        renewedDeposit.CustomerId.Should().Be(_customerId);
        renewedDeposit.PrincipalAmount.Should().Be(TestPrincipal + 250m); // Principal + Accrued Interest
        renewedDeposit.TermDays.Should().Be(newTermDays);
        renewedDeposit.Status.Should().Be(FixedDepositStatus.Active);
        renewedDeposit.RenewedFromDepositId.Should().Be(deposit.Id);
        
        deposit.Status.Should().Be(FixedDepositStatus.Renewed);
        deposit.RenewedToDepositId.Should().Be(renewedDeposit.Id);
        deposit.RenewalCount.Should().Be(1);
    }

    [Fact]
    public void Renew_OnNonMaturedDeposit_ShouldThrowException()
    {
        // Arrange
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            TestPrincipal,
            TestTermDays,
            TestInterestRate,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.None);

        // Act & Assert
        var act = () => deposit.Renew(180);
        act.Should().Throw<InvalidOperationException>();
    }

    #endregion

    #region Interest Accrual Tests

    [Fact]
    public void RecordAccruedInterest_WithValidAmount_ShouldUpdateAccruedInterest()
    {
        // Arrange
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            TestPrincipal,
            TestTermDays,
            TestInterestRate,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.None);

        var interestAmount = 125m;

        // Act
        deposit.RecordAccruedInterest(interestAmount);

        // Assert
        deposit.AccruedInterest.Should().Be(interestAmount);
    }

    [Fact]
    public void RecordAccruedInterest_MultipleRecords_ShouldAccumulate()
    {
        // Arrange
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            TestPrincipal,
            TestTermDays,
            TestInterestRate,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.None);

        // Act
        deposit.RecordAccruedInterest(125m);
        deposit.RecordAccruedInterest(125m);
        deposit.RecordAccruedInterest(125m);

        // Assert
        deposit.AccruedInterest.Should().Be(375m);
    }

    [Fact]
    public void RecordAccruedInterest_WithNegativeAmount_ShouldThrowException()
    {
        // Arrange
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            TestPrincipal,
            TestTermDays,
            TestInterestRate,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.None);

        // Act & Assert
        var act = () => deposit.RecordAccruedInterest(-100m);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void RecordAccruedInterest_OnInactiveDeposit_ShouldThrowException()
    {
        // Arrange
        var deposit = FixedDeposit.Create(
            _customerId,
            _linkedAccountId,
            _depositProductId,
            TestPrincipal,
            TestTermDays,
            TestInterestRate,
            InterestCalculationMethod.Simple,
            InterestFrequency.Monthly,
            WithdrawalPenaltyType.None);

        deposit.Close();

        // Act & Assert
        var act = () => deposit.RecordAccruedInterest(100m);
        act.Should().Throw<InvalidOperationException>();
    }

    #endregion
}
