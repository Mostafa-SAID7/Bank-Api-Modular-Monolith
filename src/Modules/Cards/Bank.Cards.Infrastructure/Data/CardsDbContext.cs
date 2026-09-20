namespace Bank.Cards.Infrastructure.Data;

using Bank.Cards.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core DbContext for Cards module
/// Schema: cards in PostgreSQL
/// Manages: Card accounts, card products, transactions, blocks
/// </summary>
public sealed class CardsDbContext : DbContext
{
    public DbSet<Card> Cards => Set<Card>();
    public DbSet<CardProduct> CardProducts => Set<CardProduct>();
    public DbSet<CardTransaction> CardTransactions => Set<CardTransaction>();
    public DbSet<CardBlock> CardBlocks => Set<CardBlock>();

    public CardsDbContext(DbContextOptions<CardsDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Set schema
        modelBuilder.HasDefaultSchema("cards");

        // Configure Card entity
        modelBuilder.Entity<Card>(entity =>
        {
            entity.ToTable("cards");
            entity.HasKey(c => c.Id);
            
            entity.Property(c => c.CardNumber)
                .HasMaxLength(255)
                .IsRequired();
            
            entity.Property(c => c.MaskedPan)
                .HasMaxLength(50)
                .IsRequired();
            
            entity.Property(c => c.CustomerId)
                .IsRequired();
            
            entity.Property(c => c.LinkedAccountId)
                .IsRequired();
            
            entity.Property(c => c.CardProductId)
                .IsRequired();
            
            entity.Property(c => c.CardType)
                .HasConversion<int>()
                .IsRequired();
            
            entity.Property(c => c.CardBrand)
                .HasConversion<int>()
                .IsRequired();
            
            entity.Property(c => c.Status)
                .HasConversion<int>()
                .IsRequired();
            
            entity.Property(c => c.HolderName)
                .HasMaxLength(100)
                .IsRequired();
            
            entity.Property(c => c.CvvHash)
                .HasMaxLength(255)
                .IsRequired();
            
            entity.Property(c => c.IssuedDateUtc)
                .IsRequired();
            
            entity.Property(c => c.ExpiryDateUtc)
                .IsRequired();
            
            entity.Property(c => c.ActivatedDateUtc)
                .IsRequired(false);
            
            entity.Property(c => c.BlockedDateUtc)
                .IsRequired(false);
            
            entity.Property(c => c.ClosedDateUtc)
                .IsRequired(false);
            
            entity.Property(c => c.DailyWithdrawalLimit)
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.Property(c => c.DailyTransactionLimit)
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.Property(c => c.DailyForeignTransactionLimit)
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.Property(c => c.DailyTransactionCount)
                .IsRequired();
            
            entity.Property(c => c.InternationalEnabled)
                .IsRequired();
            
            entity.Property(c => c.OnlineTransactionsEnabled)
                .IsRequired();
            
            entity.Property(c => c.ContactlessEnabled)
                .IsRequired();
            
            entity.Property(c => c.PinHash)
                .HasMaxLength(255)
                .IsRequired(false);
            
            entity.Property(c => c.FailedPinAttempts)
                .IsRequired();
            
            entity.Property(c => c.LastPinChangeUtc)
                .IsRequired(false);
            
            entity.Property(c => c.PinRequired)
                .IsRequired();
            
            entity.Property(c => c.PinLockedUntilUtc)
                .IsRequired(false);
            
            entity.Property(c => c.ReplacedFromCardId)
                .IsRequired(false);
            
            entity.Property(c => c.ReplacedToCardId)
                .IsRequired(false);
            
            entity.Property(c => c.ReplacementCount)
                .IsRequired();
            
            entity.Property(c => c.LimitResetDateUtc)
                .IsRequired();
            
            entity.Property(c => c.CurrentDayWithdrawalAmount)
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.Property(c => c.CurrentDayTransactionAmount)
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.Property(c => c.CreatedAtUtc)
                .IsRequired();
            
            entity.Property(c => c.UpdatedAtUtc)
                .IsRequired();

            // Indexes
            entity.HasIndex(c => c.CardNumber).IsUnique();
            entity.HasIndex(c => c.CustomerId);
            entity.HasIndex(c => c.Status);
            entity.HasIndex(c => c.ExpiryDateUtc);
            entity.HasIndex(c => c.CreatedAtUtc);
        });

        // Apply CardProduct configuration
        modelBuilder.ApplyConfiguration(new CardProductConfiguration());

        // Apply CardTransaction configuration
        modelBuilder.ApplyConfiguration(new CardTransactionConfiguration());

        // Apply CardBlock configuration
        modelBuilder.ApplyConfiguration(new CardBlockConfiguration());
    }
}
