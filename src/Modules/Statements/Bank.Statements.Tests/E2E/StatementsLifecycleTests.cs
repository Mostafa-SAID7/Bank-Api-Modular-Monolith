namespace Bank.Statements.Tests.E2E;

using Bank.Statements.Tests.Fixtures;

public sealed class StatementsLifecycleTests : IAsyncLifetime
{
    private StatementsTestFixture _fixture = null!;

    public async Task InitializeAsync()
    {
        _fixture = new StatementsTestFixture();
        await _fixture.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        if (_fixture != null)
        {
            await _fixture.DisposeAsync();
        }
    }

    #region Statement Creation Tests

    [Fact]
    public async Task CreateStatement_WithValidParameters_ShouldSucceed()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var statement = _fixture.CreateTestStatement(customerId, accountId);

        // Act
        var result = await _fixture.AddAsync(statement);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.CustomerId.Should().Be(customerId);
        result.AccountId.Should().Be(accountId);
        result.Status.Should().Be(StatementStatus.Pending);
    }

    [Fact]
    public async Task CreateStatement_WithDifferentFormats_ShouldPersistCorrectly()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var formats = new[] { StatementFormat.PDF, StatementFormat.CSV, StatementFormat.Excel, StatementFormat.HTML };

        // Act & Assert
        foreach (var format in formats)
        {
            var statement = Statement.Create(
                customerId,
                accountId,
                DateTime.UtcNow.AddDays(-30),
                DateTime.UtcNow,
                1000m,
                StatementPeriod.Monthly,
                format);

            var result = await _fixture.AddAsync(statement);
            result.PreferredFormat.Should().Be(format);
        }
    }

    [Fact]
    public async Task CreateStatement_WithDifferentPeriods_ShouldPersistCorrectly()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var periods = new[] { StatementPeriod.Daily, StatementPeriod.Weekly, StatementPeriod.Monthly, StatementPeriod.Quarterly, StatementPeriod.Yearly };

        // Act & Assert
        foreach (var period in periods)
        {
            var statement = Statement.Create(
                customerId,
                accountId,
                DateTime.UtcNow.AddDays(-30),
                DateTime.UtcNow,
                1000m,
                period,
                StatementFormat.PDF);

            var result = await _fixture.AddAsync(statement);
            result.Period.Should().Be(period);
        }
    }

    #endregion

    #region Statement Query Tests

    [Fact]
    public async Task GetStatement_WithValidId_ShouldReturnStatement()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var statement = _fixture.CreateTestStatement(customerId, accountId);
        await _fixture.AddAsync(statement);

        // Act
        var retrieved = await _fixture.DbContext.Statements
            .FirstOrDefaultAsync(s => s.Id == statement.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.CustomerId.Should().Be(customerId);
    }

    [Fact]
    public async Task GetCustomerStatements_WithMultipleStatements_ShouldReturnAll()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var statements = new List<Statement>();
        for (int i = 0; i < 5; i++)
        {
            var statement = _fixture.CreateTestStatement(customerId, Guid.NewGuid());
            statements.Add(await _fixture.AddAsync(statement));
        }

        // Act
        var retrieved = await _fixture.DbContext.Statements
            .Where(s => s.CustomerId == customerId)
            .ToListAsync();

        // Assert
        retrieved.Should().HaveCount(5);
        retrieved.Should().AllSatisfy(s => s.CustomerId.Should().Be(customerId));
    }

    [Fact]
    public async Task QueryStatementsByStatus_WithDraftStatus_ShouldReturnDraftStatements()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var draftStatement = _fixture.CreateTestStatement(customerId, Guid.NewGuid(), status: StatementStatus.Pending);
        await _fixture.AddAsync(draftStatement);

        // Act
        var retrieved = await _fixture.DbContext.Statements
            .Where(s => s.Status == StatementStatus.Pending && s.CustomerId == customerId)
            .ToListAsync();

        // Assert
        retrieved.Should().HaveCount(1);
        retrieved.First().Status.Should().Be(StatementStatus.Pending);
    }

    [Fact]
    public async Task QueryStatementsByDateRange_ShouldReturnCorrectStatements()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;

        var statement = Statement.Create(
            customerId,
            accountId,
            startDate,
            endDate,
            1000m,
            StatementPeriod.Monthly,
            StatementFormat.PDF);

        await _fixture.AddAsync(statement);

        // Act
        var retrieved = await _fixture.DbContext.Statements
            .Where(s => s.PeriodStartDate >= startDate && s.PeriodEndDate <= endDate)
            .ToListAsync();

        // Assert
        retrieved.Should().HaveCount(1);
        retrieved.First().PeriodStartDate.Should().BeOnOrAfter(startDate);
        retrieved.First().PeriodEndDate.Should().BeOnOrBefore(endDate);
    }

    #endregion

    #region Statement Recipient Tests

    [Fact]
    public async Task AddRecipient_WithValidEmail_ShouldSucceed()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var statement = _fixture.CreateTestStatement(customerId, accountId);
        await _fixture.AddAsync(statement);

        var recipient = _fixture.CreateTestRecipient(statement.Id, "recipient@example.com");

        // Act
        var result = await _fixture.AddAsync(recipient);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be("recipient@example.com");
        result.StatementId.Should().Be(statement.Id);
    }

    [Fact]
    public async Task AddMultipleRecipients_ToStatement_ShouldPersistAll()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var statement = _fixture.CreateTestStatement(customerId, accountId);
        await _fixture.AddAsync(statement);

        var emails = new[] { "recipient1@example.com", "recipient2@example.com", "recipient3@example.com" };
        var recipients = emails.Select(email => _fixture.CreateTestRecipient(statement.Id, email)).ToList();

        // Act
        foreach (var recipient in recipients)
        {
            await _fixture.AddAsync(recipient);
        }

        var retrieved = await _fixture.DbContext.StatementRecipients
            .Where(r => r.StatementId == statement.Id)
            .ToListAsync();

        // Assert
        retrieved.Should().HaveCount(3);
        retrieved.Select(r => r.Email).Should().Contain(emails);
    }

    [Fact]
    public async Task GetStatementRecipients_WithMultipleRecipients_ShouldReturnAll()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var statement = _fixture.CreateTestStatement(customerId, accountId);
        await _fixture.AddAsync(statement);

        var methods = new[] { DeliveryMethod.Email, DeliveryMethod.SMS, DeliveryMethod.Portal };
        for (int i = 0; i < methods.Length; i++)
        {
            var recipient = StatementRecipient.Create(
                statement.Id,
                $"recipient{i}@example.com",
                methods[i]);
            await _fixture.AddAsync(recipient);
        }

        // Act
        var retrieved = await _fixture.DbContext.StatementRecipients
            .Where(r => r.StatementId == statement.Id)
            .ToListAsync();

        // Assert
        retrieved.Should().HaveCount(3);
        retrieved.Select(r => r.DeliveryMethod).Should().Contain(methods);
    }

    #endregion

    #region Statement Schedule Tests

    [Fact]
    public async Task CreateSchedule_WithValidParameters_ShouldSucceed()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var schedule = _fixture.CreateTestSchedule(customerId, accountId);

        // Act
        var result = await _fixture.AddAsync(schedule);

        // Assert
        result.Should().NotBeNull();
        result.CustomerId.Should().Be(customerId);
        result.AccountId.Should().Be(accountId);
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateSchedule_WithDifferentPeriods_ShouldPersistCorrectly()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var periods = new[] { StatementPeriod.Weekly, StatementPeriod.Monthly, StatementPeriod.Quarterly };

        // Act & Assert
        foreach (var period in periods)
        {
            var schedule = StatementSchedule.Create(
                customerId,
                accountId,
                period,
                StatementFormat.PDF,
                DeliveryMethod.Email);

            var result = await _fixture.AddAsync(schedule);
            result.Period.Should().Be(period);
        }
    }

    [Fact]
    public async Task CreateSchedule_WithDifferentDeliveryMethods_ShouldPersistCorrectly()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var methods = new[] { DeliveryMethod.Email, DeliveryMethod.SMS, DeliveryMethod.Portal };

        // Act & Assert
        foreach (var method in methods)
        {
            var schedule = StatementSchedule.Create(
                customerId,
                accountId,
                StatementPeriod.Monthly,
                StatementFormat.PDF,
                method);

            var result = await _fixture.AddAsync(schedule);
            result.DeliveryMethod.Should().Be(method);
        }
    }

    [Fact]
    public async Task GetAccountSchedule_WithValidId_ShouldReturnSchedule()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var schedule = _fixture.CreateTestSchedule(customerId, accountId);
        await _fixture.AddAsync(schedule);

        // Act
        var retrieved = await _fixture.DbContext.StatementSchedules
            .FirstOrDefaultAsync(s => s.Id == schedule.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.CustomerId.Should().Be(customerId);
        retrieved.AccountId.Should().Be(accountId);
    }

    [Fact]
    public async Task GetActiveSchedules_ForAccount_ShouldReturnActiveOnly()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var activeSchedule = _fixture.CreateTestSchedule(customerId, accountId);
        await _fixture.AddAsync(activeSchedule);

        // Act
        var retrieved = await _fixture.DbContext.StatementSchedules
            .Where(s => s.AccountId == accountId && s.IsActive)
            .ToListAsync();

        // Assert
        retrieved.Should().HaveCount(1);
        retrieved.First().IsActive.Should().BeTrue();
    }

    #endregion

    #region Statement Lifecycle Tests

    [Fact]
    public async Task StatementLifecycle_GenerateToSend_ShouldSucceed()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var statement = _fixture.CreateTestStatement(customerId, accountId);
        await _fixture.AddAsync(statement);

        // Add recipients
        var recipient1 = _fixture.CreateTestRecipient(statement.Id, "recipient1@example.com");
        var recipient2 = _fixture.CreateTestRecipient(statement.Id, "recipient2@example.com", DeliveryMethod.SMS);
        await _fixture.AddAsync(recipient1);
        await _fixture.AddAsync(recipient2);

        // Act - Generate
        statement.Generate(new List<StatementLine>(), 500m, 250m);
        await _fixture.DbContext.SaveChangesAsync();

        // Assert - Statement generated
        var retrievedStatement = await _fixture.DbContext.Statements
            .FirstOrDefaultAsync(s => s.Id == statement.Id);
        retrievedStatement!.Status.Should().Be(StatementStatus.Generated);

        // Get recipients
        var recipients = await _fixture.DbContext.StatementRecipients
            .Where(r => r.StatementId == statement.Id)
            .ToListAsync();
        recipients.Should().HaveCount(2);
    }

    [Fact]
    public async Task StatementLifecycle_MultipleOperations_ShouldMaintainConsistency()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();

        // Create statement
        var statement = _fixture.CreateTestStatement(customerId, accountId);
        await _fixture.AddAsync(statement);
        statement.Id.Should().NotBeEmpty();

        // Add multiple recipients
        var recipientEmails = new[] { "r1@test.com", "r2@test.com", "r3@test.com" };
        foreach (var email in recipientEmails)
        {
            var recipient = _fixture.CreateTestRecipient(statement.Id, email);
            await _fixture.AddAsync(recipient);
        }

        // Create schedule
        var schedule = _fixture.CreateTestSchedule(customerId, accountId);
        await _fixture.AddAsync(schedule);

        // Act - Query all related entities
        var stmtCount = await _fixture.DbContext.Statements.CountAsync(s => s.CustomerId == customerId);
        var recipients = await _fixture.DbContext.StatementRecipients
            .Where(r => r.StatementId == statement.Id)
            .ToListAsync();
        var schedules = await _fixture.DbContext.StatementSchedules
            .Where(s => s.CustomerId == customerId)
            .ToListAsync();

        // Assert
        stmtCount.Should().Be(1);
        recipients.Should().HaveCount(3);
        schedules.Should().HaveCount(1);
    }

    #endregion

    #region Data Integrity Tests

    [Fact]
    public async Task Statement_WithNoRecipients_ShouldBePersisted()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var statement = _fixture.CreateTestStatement(customerId, accountId);

        // Act
        await _fixture.AddAsync(statement);

        var retrieved = await _fixture.DbContext.Statements
            .Include(s => s.Recipients)
            .FirstOrDefaultAsync(s => s.Id == statement.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Recipients.Should().BeEmpty();
    }

    [Fact]
    public async Task MultipleStatements_ForDifferentCustomers_ShouldBeIsolated()
    {
        // Arrange
        var customer1 = Guid.NewGuid();
        var customer2 = Guid.NewGuid();

        var stmt1 = _fixture.CreateTestStatement(customer1, Guid.NewGuid());
        var stmt2 = _fixture.CreateTestStatement(customer2, Guid.NewGuid());

        await _fixture.AddAsync(stmt1);
        await _fixture.AddAsync(stmt2);

        // Act
        var customer1Statements = await _fixture.DbContext.Statements
            .Where(s => s.CustomerId == customer1)
            .ToListAsync();

        var customer2Statements = await _fixture.DbContext.Statements
            .Where(s => s.CustomerId == customer2)
            .ToListAsync();

        // Assert
        customer1Statements.Should().HaveCount(1);
        customer2Statements.Should().HaveCount(1);
        customer1Statements.First().CustomerId.Should().Be(customer1);
        customer2Statements.First().CustomerId.Should().Be(customer2);
    }

    [Fact]
    public async Task Statement_Dates_ShouldBeConsistent()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;

        var statement = Statement.Create(
            customerId,
            accountId,
            startDate,
            endDate,
            1000m,
            StatementPeriod.Monthly,
            StatementFormat.PDF);

        await _fixture.AddAsync(statement);

        // Act
        var retrieved = await _fixture.DbContext.Statements
            .FirstOrDefaultAsync(s => s.Id == statement.Id);

        // Assert
        retrieved!.PeriodStartDate.Should().Be(startDate);
        retrieved.PeriodEndDate.Should().Be(endDate);
        retrieved.PeriodEndDate.Should().BeAfter(retrieved.PeriodStartDate);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task QueryNonExistentStatement_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _fixture.DbContext.Statements
            .FirstOrDefaultAsync(s => s.Id == nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetStatementsForNonExistentCustomer_ShouldReturnEmpty()
    {
        // Arrange
        var nonExistentCustomerId = Guid.NewGuid();

        // Act
        var results = await _fixture.DbContext.Statements
            .Where(s => s.CustomerId == nonExistentCustomerId)
            .ToListAsync();

        // Assert
        results.Should().BeEmpty();
    }

    #endregion
}
