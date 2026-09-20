namespace Bank.Cards.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for CardProduct entity
/// </summary>
public sealed class CardProductConfiguration : IEntityTypeConfiguration<CardProduct>
{
    public void Configure(EntityTypeBuilder<CardProduct> builder)
    {
        builder.ToTable("card_products");
        builder.HasKey(cp => cp.Id);

        builder.Property(cp => cp.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(cp => cp.Description)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(cp => cp.CardType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(cp => cp.Brand)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(cp => cp.AnnualFee)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(cp => cp.IssuanceFeePercentage)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(cp => cp.AtmFee)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(cp => cp.ForeignTransactionFeePercentage)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(cp => cp.DefaultDailyLimit)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(cp => cp.DefaultTransactionLimit)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(cp => cp.DailyTransactionCountLimit)
            .IsRequired();

        builder.Property(cp => cp.RequiresPinForAtm)
            .IsRequired();

        builder.Property(cp => cp.AllowsContactless)
            .IsRequired();

        builder.Property(cp => cp.AllowsOnlineTransactions)
            .IsRequired();

        builder.Property(cp => cp.AllowsInternational)
            .IsRequired();

        builder.Property(cp => cp.RequiresPinForContactless)
            .IsRequired();

        builder.Property(cp => cp.CardValidityYears)
            .IsRequired();

        builder.Property(cp => cp.ReplacementAllowancePerYear)
            .IsRequired();

        builder.Property(cp => cp.IsActive)
            .IsRequired();

        builder.Property(cp => cp.CreatedAtUtc)
            .IsRequired();

        builder.Property(cp => cp.UpdatedAtUtc)
            .IsRequired();

        // Indexes
        builder.HasIndex(cp => cp.Name).IsUnique();
        builder.HasIndex(cp => cp.IsActive);
    }
}
