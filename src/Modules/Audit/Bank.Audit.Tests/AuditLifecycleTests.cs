namespace Bank.Audit.Tests;

public sealed class AuditLifecycleTests : IAsyncLifetime
{
    private AuditTestFixture _fixture = null!;

    public async Task InitializeAsync()
    {
        _fixture = new AuditTestFixture();
        await _fixture.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        await _fixture.DisposeAsync();
    }

    [Fact]
    public async Task CreateAuditLog_WithValidData_ShouldPersist()
    {
        // Arrange
        var auditLog = AuditLog.Create(
            userId: "user123",
            eventType: AuditEventType.Create,
            resourceType: ResourceType.Account,
            resourceId: "acc456",
            action: "Created account",
            timestamp: DateTime.UtcNow,
            ipAddress: "192.168.1.1",
            userAgent: "Mozilla/5.0",
            details: "Test audit log creation");

        // Act
        _fixture.DbContext.AuditLogs.Add(auditLog);
        await _fixture.DbContext.SaveChangesAsync();

        // Assert
        var retrieved = await _fixture.DbContext.AuditLogs.FirstOrDefaultAsync(a => a.Id == auditLog.Id);
        Assert.NotNull(retrieved);
        Assert.Equal("user123", retrieved.UserId);
        Assert.Equal(AuditEventType.Create, retrieved.EventType);
    }

    [Fact]
    public async Task CreateAuditLog_WithMultipleEvents_ShouldPersistAll()
    {
        // Arrange
        var logs = new[]
        {
            AuditLog.Create("user1", AuditEventType.Create, ResourceType.Account, "acc1", "Created", DateTime.UtcNow, "192.168.1.1", "Mozilla", null),
            AuditLog.Create("user1", AuditEventType.Update, ResourceType.Account, "acc1", "Updated", DateTime.UtcNow, "192.168.1.1", "Mozilla", null),
            AuditLog.Create("user1", AuditEventType.Delete, ResourceType.Account, "acc1", "Deleted", DateTime.UtcNow, "192.168.1.1", "Mozilla", null)
        };

        // Act
        _fixture.DbContext.AuditLogs.AddRange(logs);
        await _fixture.DbContext.SaveChangesAsync();

        // Assert
        var allLogs = await _fixture.DbContext.AuditLogs.Where(a => a.UserId == "user1").ToListAsync();
        Assert.Equal(3, allLogs.Count);
    }

    [Fact]
    public async Task ArchiveAuditLog_ShouldChangeStatus()
    {
        // Arrange
        var auditLog = AuditLog.Create("user123", AuditEventType.Create, ResourceType.Account, "acc456", "Created", DateTime.UtcNow, "192.168.1.1", "Mozilla", null);
        _fixture.DbContext.AuditLogs.Add(auditLog);
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        auditLog.Archive();
        await _fixture.DbContext.SaveChangesAsync();

        // Assert
        var retrieved = await _fixture.DbContext.AuditLogs.FirstOrDefaultAsync(a => a.Id == auditLog.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(AuditStatus.Archived, retrieved.Status);
    }

    [Fact]
    public async Task CreateAuditEntry_WithValidData_ShouldPersist()
    {
        // Arrange
        var auditLog = AuditLog.Create("user123", AuditEventType.Create, ResourceType.Account, "acc456", "Created", DateTime.UtcNow, "192.168.1.1", "Mozilla", null);
        _fixture.DbContext.AuditLogs.Add(auditLog);
        await _fixture.DbContext.SaveChangesAsync();

        var entry = AuditEntry.Create(auditLog.Id, "Account", ActionType.Insert, "{}", "{\"id\": \"acc456\"}", EntityChangeType.Added);

        // Act
        _fixture.DbContext.AuditEntries.Add(entry);
        await _fixture.DbContext.SaveChangesAsync();

        // Assert
        var retrieved = await _fixture.DbContext.AuditEntries.FirstOrDefaultAsync(e => e.Id == entry.Id);
        Assert.NotNull(retrieved);
        Assert.Equal("Account", retrieved.EntityName);
        Assert.Equal(ActionType.Insert, retrieved.ActionType);
    }

    [Fact]
    public async Task CreateAuditEntry_MultipleByAuditLog_ShouldQueryByLog()
    {
        // Arrange
        var auditLog = AuditLog.Create("user123", AuditEventType.Update, ResourceType.Account, "acc456", "Updated", DateTime.UtcNow, "192.168.1.1", "Mozilla", null);
        _fixture.DbContext.AuditLogs.Add(auditLog);
        await _fixture.DbContext.SaveChangesAsync();

        var entries = new[]
        {
            AuditEntry.Create(auditLog.Id, "Account", ActionType.Update, "{\"name\": \"Old\"}", "{\"name\": \"New\"}", EntityChangeType.Modified),
            AuditEntry.Create(auditLog.Id, "Account", ActionType.Update, "{\"balance\": \"100\"}", "{\"balance\": \"200\"}", EntityChangeType.Modified)
        };
        _fixture.DbContext.AuditEntries.AddRange(entries);
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        var retrieved = await _fixture.DbContext.AuditEntries
            .Where(e => e.AuditLogId == auditLog.Id)
            .ToListAsync();

        // Assert
        Assert.Equal(2, retrieved.Count);
    }

    [Fact]
    public async Task CreateAuditTrail_WithValidData_ShouldPersist()
    {
        // Arrange
        var trail = AuditTrail.Create(
            "Trail-2026-01",
            new DateTime(2026, 1, 1),
            new DateTime(2026, 1, 31),
            100,
            "/exports/trail-2026-01.pdf",
            "PDF");

        // Act
        _fixture.DbContext.AuditTrails.Add(trail);
        await _fixture.DbContext.SaveChangesAsync();

        // Assert
        var retrieved = await _fixture.DbContext.AuditTrails.FirstOrDefaultAsync(t => t.Id == trail.Id);
        Assert.NotNull(retrieved);
        Assert.Equal("Trail-2026-01", retrieved.TrailName);
        Assert.Equal(100, retrieved.TotalEntries);
    }

    [Fact]
    public async Task CreateAuditConfiguration_WithValidData_ShouldPersist()
    {
        // Arrange
        var config = AuditConfiguration.Create(
            AuditLevel.Detailed,
            90,
            true,
            "audit@example.com",
            "admin");

        // Act
        _fixture.DbContext.AuditConfigurations.Add(config);
        await _fixture.DbContext.SaveChangesAsync();

        // Assert
        var retrieved = await _fixture.DbContext.AuditConfigurations.FirstOrDefaultAsync(c => c.Id == config.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(AuditLevel.Detailed, retrieved.AuditLevel);
        Assert.Equal(90, retrieved.RetentionDays);
    }

    [Fact]
    public async Task UpdateAuditConfiguration_ShouldPersistChanges()
    {
        // Arrange
        var config = AuditConfiguration.Create(AuditLevel.Standard, 30, false, "", "admin");
        _fixture.DbContext.AuditConfigurations.Add(config);
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        config.UpdateLevel(AuditLevel.Comprehensive, "admin");
        config.UpdateRetentionPolicy(60, "admin");
        await _fixture.DbContext.SaveChangesAsync();

        // Assert
        var retrieved = await _fixture.DbContext.AuditConfigurations.FirstOrDefaultAsync(c => c.Id == config.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(AuditLevel.Comprehensive, retrieved.AuditLevel);
        Assert.Equal(60, retrieved.RetentionDays);
    }

    [Fact]
    public async Task CreateUserAuditContext_WithValidData_ShouldPersist()
    {
        // Arrange
        var context = UserAuditContext.Create("user123", "Initial login");

        // Act
        _fixture.DbContext.UserAuditContexts.Add(context);
        await _fixture.DbContext.SaveChangesAsync();

        // Assert
        var retrieved = await _fixture.DbContext.UserAuditContexts.FirstOrDefaultAsync(c => c.Id == context.Id);
        Assert.NotNull(retrieved);
        Assert.Equal("user123", retrieved.UserId);
        Assert.True(retrieved.IsActive);
    }

    [Fact]
    public async Task UserAuditContext_IncrementCount_ShouldUpdate()
    {
        // Arrange
        var context = UserAuditContext.Create("user123", "Initial");
        _fixture.DbContext.UserAuditContexts.Add(context);
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        context.IncrementAuditLogCount();
        context.IncrementAuditLogCount();
        await _fixture.DbContext.SaveChangesAsync();

        // Assert
        var retrieved = await _fixture.DbContext.UserAuditContexts.FirstOrDefaultAsync(c => c.Id == context.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(2, retrieved.AuditLogCount);
    }

    [Fact]
    public async Task QueryAuditLogsByUser_ShouldReturnCorrectLogs()
    {
        // Arrange
        var user1Logs = new[] { 
            AuditLog.Create("user1", AuditEventType.Create, ResourceType.Account, "acc1", "C1", DateTime.UtcNow, "192.168.1.1", "Mozilla", null),
            AuditLog.Create("user1", AuditEventType.Update, ResourceType.Account, "acc1", "U1", DateTime.UtcNow, "192.168.1.1", "Mozilla", null)
        };
        var user2Log = AuditLog.Create("user2", AuditEventType.Delete, ResourceType.Account, "acc2", "D1", DateTime.UtcNow, "192.168.1.1", "Mozilla", null);
        
        _fixture.DbContext.AuditLogs.AddRange(user1Logs);
        _fixture.DbContext.AuditLogs.Add(user2Log);
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        var retrieved = await _fixture.DbContext.AuditLogs
            .Where(l => l.UserId == "user1")
            .ToListAsync();

        // Assert
        Assert.Equal(2, retrieved.Count);
        Assert.All(retrieved, log => Assert.Equal("user1", log.UserId));
    }

    [Fact]
    public async Task QueryAuditLogsByDateRange_ShouldReturnInRange()
    {
        // Arrange
        var start = new DateTime(2026, 1, 10);
        var end = new DateTime(2026, 1, 20);
        
        _fixture.DbContext.AuditLogs.Add(AuditLog.Create("user1", AuditEventType.Create, ResourceType.Account, "acc1", "Early", new DateTime(2026, 1, 5), "192.168.1.1", "Mozilla", null));
        _fixture.DbContext.AuditLogs.Add(AuditLog.Create("user1", AuditEventType.Create, ResourceType.Account, "acc2", "InRange1", new DateTime(2026, 1, 15), "192.168.1.1", "Mozilla", null));
        _fixture.DbContext.AuditLogs.Add(AuditLog.Create("user1", AuditEventType.Create, ResourceType.Account, "acc3", "InRange2", new DateTime(2026, 1, 18), "192.168.1.1", "Mozilla", null));
        _fixture.DbContext.AuditLogs.Add(AuditLog.Create("user1", AuditEventType.Create, ResourceType.Account, "acc4", "Late", new DateTime(2026, 1, 25), "192.168.1.1", "Mozilla", null));
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        var retrieved = await _fixture.DbContext.AuditLogs
            .Where(l => l.Timestamp >= start && l.Timestamp <= end)
            .ToListAsync();

        // Assert
        Assert.Equal(2, retrieved.Count);
        Assert.All(retrieved, log => Assert.Contains(new[] { "InRange1", "InRange2" }, a => a == log.Action));
    }

    [Fact]
    public async Task FilterAuditLogsByStatus_ShouldReturnActive()
    {
        // Arrange
        var active = AuditLog.Create("user1", AuditEventType.Create, ResourceType.Account, "acc1", "Active", DateTime.UtcNow, "192.168.1.1", "Mozilla", null);
        var archived = AuditLog.Create("user1", AuditEventType.Create, ResourceType.Account, "acc2", "Archived", DateTime.UtcNow, "192.168.1.1", "Mozilla", null);
        
        _fixture.DbContext.AuditLogs.Add(active);
        _fixture.DbContext.AuditLogs.Add(archived);
        await _fixture.DbContext.SaveChangesAsync();
        
        archived.Archive();
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        var activeLogs = await _fixture.DbContext.AuditLogs
            .Where(l => l.Status == AuditStatus.Active)
            .ToListAsync();

        // Assert
        Assert.Single(activeLogs);
        Assert.Equal("Active", activeLogs[0].Action);
    }

    [Fact]
    public async Task AuditLog_MultipleResourceTypes_ShouldAllPersist()
    {
        // Arrange
        var logs = new[]
        {
            AuditLog.Create("user1", AuditEventType.Create, ResourceType.Account, "acc1", "A1", DateTime.UtcNow, "192.168.1.1", "Mozilla", null),
            AuditLog.Create("user1", AuditEventType.Create, ResourceType.Transaction, "tx1", "T1", DateTime.UtcNow, "192.168.1.1", "Mozilla", null),
            AuditLog.Create("user1", AuditEventType.Create, ResourceType.User, "usr1", "U1", DateTime.UtcNow, "192.168.1.1", "Mozilla", null),
            AuditLog.Create("user1", AuditEventType.Create, ResourceType.Role, "role1", "R1", DateTime.UtcNow, "192.168.1.1", "Mozilla", null)
        };
        _fixture.DbContext.AuditLogs.AddRange(logs);
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        var allLogs = await _fixture.DbContext.AuditLogs.ToListAsync();

        // Assert
        Assert.Equal(4, allLogs.Count);
        Assert.Contains(allLogs, l => l.ResourceType == ResourceType.Account);
        Assert.Contains(allLogs, l => l.ResourceType == ResourceType.Transaction);
        Assert.Contains(allLogs, l => l.ResourceType == ResourceType.User);
        Assert.Contains(allLogs, l => l.ResourceType == ResourceType.Role);
    }

    [Fact]
    public async Task AuditEntry_WithLargeOldNewValues_ShouldPersist()
    {
        // Arrange
        var auditLog = AuditLog.Create("user1", AuditEventType.Update, ResourceType.Account, "acc1", "Updated", DateTime.UtcNow, "192.168.1.1", "Mozilla", null);
        _fixture.DbContext.AuditLogs.Add(auditLog);
        await _fixture.DbContext.SaveChangesAsync();

        var largeOldValues = string.Concat(Enumerable.Repeat("old_data_", 100));
        var largeNewValues = string.Concat(Enumerable.Repeat("new_data_", 100));
        
        var entry = AuditEntry.Create(auditLog.Id, "Account", ActionType.Update, largeOldValues, largeNewValues, EntityChangeType.Modified);

        // Act
        _fixture.DbContext.AuditEntries.Add(entry);
        await _fixture.DbContext.SaveChangesAsync();

        // Assert
        var retrieved = await _fixture.DbContext.AuditEntries.FirstOrDefaultAsync(e => e.Id == entry.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(largeOldValues, retrieved.OldValues);
        Assert.Equal(largeNewValues, retrieved.NewValues);
    }

    [Fact]
    public async Task DeleteAuditEntry_ShouldRemove()
    {
        // Arrange
        var auditLog = AuditLog.Create("user1", AuditEventType.Create, ResourceType.Account, "acc1", "Created", DateTime.UtcNow, "192.168.1.1", "Mozilla", null);
        _fixture.DbContext.AuditLogs.Add(auditLog);
        await _fixture.DbContext.SaveChangesAsync();

        var entry = AuditEntry.Create(auditLog.Id, "Account", ActionType.Insert, "{}", "{}", EntityChangeType.Added);
        _fixture.DbContext.AuditEntries.Add(entry);
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        _fixture.DbContext.AuditEntries.Remove(entry);
        await _fixture.DbContext.SaveChangesAsync();

        // Assert
        var retrieved = await _fixture.DbContext.AuditEntries.FirstOrDefaultAsync(e => e.Id == entry.Id);
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task Transaction_RollbackOnError_ShouldNotPersist()
    {
        // Arrange
        var auditLog = AuditLog.Create("user1", AuditEventType.Create, ResourceType.Account, "acc1", "Created", DateTime.UtcNow, "192.168.1.1", "Mozilla", null);

        // Act & Assert
        try
        {
            using var transaction = await _fixture.DbContext.Database.BeginTransactionAsync();
            _fixture.DbContext.AuditLogs.Add(auditLog);
            await _fixture.DbContext.SaveChangesAsync();
            throw new InvalidOperationException("Simulated error");
            #pragma warning disable CS0162
            await transaction.CommitAsync();
            #pragma warning restore CS0162
        }
        catch (InvalidOperationException)
        {
            // Expected
        }

        var retrieved = await _fixture.DbContext.AuditLogs.FirstOrDefaultAsync(a => a.Id == auditLog.Id);
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task ConcurrentInserts_ShouldAllSucceed()
    {
        // Arrange
        var tasks = Enumerable.Range(1, 10)
            .Select(i => Task.Run(async () =>
            {
                var log = AuditLog.Create($"user{i}", AuditEventType.Create, ResourceType.Account, $"acc{i}", $"Action{i}", DateTime.UtcNow, "192.168.1.1", "Mozilla", null);
                _fixture.DbContext.AuditLogs.Add(log);
                await _fixture.DbContext.SaveChangesAsync();
            }))
            .ToArray();

        // Act
        await Task.WhenAll(tasks);

        // Assert
        var allLogs = await _fixture.DbContext.AuditLogs.ToListAsync();
        Assert.Equal(10, allLogs.Count);
    }
}
