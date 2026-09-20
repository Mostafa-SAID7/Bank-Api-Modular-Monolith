namespace Bank.Deposits.Infrastructure.Data;

/// <summary>
/// EF Core DbContext for Deposits module
/// Schema: deposits in PostgreSQL
/// Manages: Deposit accounts, deposit types, interest rates
/// </summary>
public sealed class DepositsDbContext : DbContext
{
    public DbSet<Deposit> Deposits => Set<Deposit>();
    public DbSet<DepositType> DepositTypes => Set<DepositType>();
    public DbSet<InterestRate> InterestRates => Set<InterestRate>();
    public DbSet<FixedDeposit> FixedDeposits => Set<FixedDeposit>();
    public DbSet<DepositProduct> DepositProducts => Set<DepositProduct>();
    public DbSet<DepositTransaction> DepositTransactions => Set<DepositTransaction>();
    public DbSet<MaturityNotice> MaturityNotices => Set<MaturityNotice>();

    public DepositsDbContext(DbContextOptions<DepositsDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Set schema
        modelBuilder.HasDefaultSchema("deposits");

        // Configure Deposit entity
        modelBuilder.Entity<Deposit>(entity =>
        {
            entity.ToTable("deposits");
            entity.HasKey(d => d.Id);
            
            entity.Property(d => d.AccountNumber)
                .HasMaxLength(50)
                .IsRequired();
            
            entity.Property(d => d.CustomerId)
                .IsRequired();
            
            entity.Property(d => d.DepositTypeId)
                .IsRequired();
            
            entity.Property(d => d.CurrentBalance)
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.Property(d => d.AccruedInterest)
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.Property(d => d.PaidInterest)
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.Property(d => d.Status)
                .HasConversion<int>()
                .IsRequired();
            
            entity.Property(d => d.OpenedAtUtc)
                .IsRequired();
            
            entity.Property(d => d.MaturityDateUtc)
                .IsRequired(false);
            
            entity.Property(d => d.ClosedAtUtc)
                .IsRequired(false);
            
            entity.Property(d => d.LastInterestAccrualUtc)
                .IsRequired(false);
            
            entity.Property(d => d.NextInterestPaymentUtc)
                .IsRequired(false);
            
            entity.Property(d => d.IsFixedDeposit)
                .IsRequired();
            
            entity.Property(d => d.TermMonths)
                .IsRequired(false);
            
            entity.Property(d => d.InterestFrequency)
                .HasConversion<int>()
                .IsRequired(false);
            
            entity.Property(d => d.FreezeReason)
                .HasMaxLength(500)
                .IsRequired(false);
            
            entity.Property(d => d.ClosureReason)
                .HasMaxLength(500)
                .IsRequired(false);
            
            entity.Property(d => d.UpdatedAtUtc)
                .IsRequired();

            // Indexes
            entity.HasIndex(d => d.AccountNumber).IsUnique();
            entity.HasIndex(d => d.CustomerId);
            entity.HasIndex(d => d.Status);
            entity.HasIndex(d => d.OpenedAtUtc);
            entity.HasIndex(d => d.MaturityDateUtc);
        });

        // Configure DepositType entity
        modelBuilder.Entity<DepositType>(entity =>
        {
            entity.ToTable("deposit_types");
            entity.HasKey(dt => dt.Id);
            
            entity.Property(dt => dt.Name)
                .HasMaxLength(100)
                .IsRequired();
            
            entity.Property(dt => dt.Description)
                .HasMaxLength(500)
                .IsRequired(false);
            
            entity.Property(dt => dt.MinimumBalance)
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.Property(dt => dt.MaximumBalance)
                .HasPrecision(18, 2)
                .IsRequired(false);
            
            entity.Property(dt => dt.IsFixedDeposit)
                .IsRequired();
            
            entity.Property(dt => dt.DefaultTermMonths)
                .IsRequired(false);
            
            entity.Property(dt => dt.AllowsEarlyWithdrawal)
                .IsRequired();
            
            entity.Property(dt => dt.EarlyWithdrawalPenaltyPercent)
                .HasPrecision(5, 2)
                .IsRequired(false);
            
            entity.Property(dt => dt.AllowsPrematureClosure)
                .IsRequired();
            
            entity.Property(dt => dt.PrematureClosurePenaltyPercent)
                .HasPrecision(5, 2)
                .IsRequired(false);
            
            entity.Property(dt => dt.IsActive)
                .IsRequired();
            
            entity.Property(dt => dt.CreatedAtUtc)
                .IsRequired();
            
            entity.Property(dt => dt.UpdatedAtUtc)
                .IsRequired(false);

            // Indexes
            entity.HasIndex(dt => dt.Name).IsUnique();
            entity.HasIndex(dt => dt.IsActive);
        });

        // Configure InterestRate entity
        modelBuilder.Entity<InterestRate>(entity =>
        {
            entity.ToTable("interest_rates");
            entity.HasKey(ir => ir.Id);
            
            entity.Property(ir => ir.DepositTypeId)
                .IsRequired();
            
            entity.Property(ir => ir.AnnualRate)
                .HasPrecision(5, 2)
                .IsRequired();
            
            entity.Property(ir => ir.EffectiveFromUtc)
                .IsRequired();
            
            entity.Property(ir => ir.EffectiveToUtc)
                .IsRequired(false);
            
            entity.Property(ir => ir.MinimumBalanceForRate)
                .HasPrecision(18, 2)
                .IsRequired(false);
            
            entity.Property(ir => ir.MaximumBalanceForRate)
                .HasPrecision(18, 2)
                .IsRequired(false);
            
            entity.Property(ir => ir.Frequency)
                .HasConversion<int>()
                .IsRequired();
            
            entity.Property(ir => ir.CalculationMethod)
                .HasConversion<int>()
                .IsRequired();
            
            entity.Property(ir => ir.IsActive)
                .IsRequired();
            
            entity.Property(ir => ir.CreatedAtUtc)
                .IsRequired();
            
            entity.Property(ir => ir.UpdatedAtUtc)
                .IsRequired(false);

            // Indexes
            entity.HasIndex(ir => ir.DepositTypeId);
            entity.HasIndex(ir => ir.EffectiveFromUtc);
            entity.HasIndex(ir => ir.IsActive);
        });

        // Apply FixedDeposit configuration
        modelBuilder.ApplyConfiguration(new FixedDepositConfiguration());

        // Apply DepositProduct configuration
        modelBuilder.ApplyConfiguration(new DepositProductConfiguration());

        // Apply DepositTransaction configuration
        modelBuilder.ApplyConfiguration(new DepositTransactionConfiguration());

        // Apply MaturityNotice configuration
        modelBuilder.ApplyConfiguration(new MaturityNoticeConfiguration());
    }
}
