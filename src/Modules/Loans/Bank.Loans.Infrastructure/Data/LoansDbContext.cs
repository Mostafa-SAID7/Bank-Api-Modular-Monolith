namespace Bank.Loans.Infrastructure.Data;

/// <summary>
/// EF Core DbContext for Loans module
/// Schema: loans in PostgreSQL
/// Manages: Loan accounts, loan products, loan schedules, EMI tracking
/// </summary>
public sealed class LoansDbContext : DbContext
{
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<LoanProduct> LoanProducts => Set<LoanProduct>();
    public DbSet<LoanSchedule> LoanSchedules => Set<LoanSchedule>();

    public LoansDbContext(DbContextOptions<LoansDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Set schema
        modelBuilder.HasDefaultSchema("loans");

        // Configure Loan entity (aggregate root)
        modelBuilder.Entity<Loan>(entity =>
        {
            entity.ToTable("loans");
            entity.HasKey(l => l.Id);
            
            entity.Property(l => l.LoanNumber)
                .HasMaxLength(50)
                .IsRequired();
            
            entity.Property(l => l.CustomerId)
                .IsRequired();
            
            entity.Property(l => l.LoanProductId)
                .IsRequired();
            
            entity.Property(l => l.LoanAmount)
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.Property(l => l.ProcessingFee)
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.Property(l => l.NetDisbursedAmount)
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.Property(l => l.TenureMonths)
                .IsRequired();
            
            entity.Property(l => l.AnnualInterestRate)
                .HasPrecision(5, 2)
                .IsRequired();
            
            entity.Property(l => l.EMIAmount)
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.Property(l => l.OutstandingPrincipal)
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.Property(l => l.OutstandingInterest)
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.Property(l => l.TotalPrincipalRepaid)
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.Property(l => l.TotalInterestRepaid)
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.Property(l => l.TotalPenaltyCharges)
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.Property(l => l.Status)
                .HasConversion<int>()
                .IsRequired();
            
            entity.Property(l => l.ApplicationDateUtc)
                .IsRequired();
            
            entity.Property(l => l.ApprovalDateUtc)
                .IsRequired(false);
            
            entity.Property(l => l.DisbursementDateUtc)
                .IsRequired(false);
            
            entity.Property(l => l.ClosureDateUtc)
                .IsRequired(false);
            
            entity.Property(l => l.RejectionDateUtc)
                .IsRequired(false);
            
            entity.Property(l => l.CollateralValue)
                .HasPrecision(18, 2)
                .IsRequired(false);
            
            entity.Property(l => l.CollateralDescription)
                .HasMaxLength(500)
                .IsRequired(false);
            
            entity.Property(l => l.AllowsPrepayment)
                .IsRequired();
            
            entity.Property(l => l.PrepaymentPenaltyPercent)
                .HasPrecision(5, 2)
                .IsRequired(false);
            
            entity.Property(l => l.ApprovedByUserId)
                .IsRequired(false);
            
            entity.Property(l => l.ApprovalNotes)
                .HasMaxLength(1000)
                .IsRequired(false);
            
            entity.Property(l => l.UpdatedAtUtc)
                .IsRequired();

            // Indexes
            entity.HasIndex(l => l.LoanNumber).IsUnique();
            entity.HasIndex(l => l.CustomerId);
            entity.HasIndex(l => l.Status);
            entity.HasIndex(l => l.ApplicationDateUtc);
            entity.HasIndex(l => l.ApprovalDateUtc);
        });

        // Configure LoanProduct entity
        modelBuilder.Entity<LoanProduct>(entity =>
        {
            entity.ToTable("loan_products");
            entity.HasKey(lp => lp.Id);
            
            entity.Property(lp => lp.Name)
                .HasMaxLength(100)
                .IsRequired();
            
            entity.Property(lp => lp.Description)
                .HasMaxLength(500)
                .IsRequired(false);
            
            entity.Property(lp => lp.Type)
                .HasConversion<int>()
                .IsRequired();
            
            entity.Property(lp => lp.MinimumAmount)
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.Property(lp => lp.MaximumAmount)
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.Property(lp => lp.MinimumTenureMonths)
                .IsRequired();
            
            entity.Property(lp => lp.MaximumTenureMonths)
                .IsRequired();
            
            entity.Property(lp => lp.BaseAnnualInterestRate)
                .HasPrecision(5, 2)
                .IsRequired();
            
            entity.Property(lp => lp.InterestType)
                .HasConversion<int>()
                .IsRequired();
            
            entity.Property(lp => lp.EMIFrequency)
                .HasConversion<int>()
                .IsRequired();

            // Indexes
            entity.HasIndex(lp => lp.Name).IsUnique();
            entity.HasIndex(lp => lp.Type);
        });

        // Configure LoanSchedule entity
        modelBuilder.Entity<LoanSchedule>(entity =>
        {
            entity.ToTable("loan_schedules");
            entity.HasKey(ls => ls.Id);
            
            entity.Property(ls => ls.LoanId)
                .IsRequired();
            
            entity.Property(ls => ls.InstallmentNumber)
                .IsRequired();
            
            entity.Property(ls => ls.DueDateUtc)
                .IsRequired();
            
            entity.Property(ls => ls.PrincipalAmount)
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.Property(ls => ls.InterestAmount)
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.Property(ls => ls.TotalAmount)
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.Property(ls => ls.OutstandingPrincipal)
                .HasPrecision(18, 2)
                .IsRequired();

            // Indexes
            entity.HasIndex(ls => ls.LoanId);
            entity.HasIndex(ls => ls.DueDateUtc);
        });
    }
}
