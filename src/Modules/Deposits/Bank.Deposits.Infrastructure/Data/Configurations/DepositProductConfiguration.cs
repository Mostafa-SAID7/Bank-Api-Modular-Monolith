namespace Bank.Deposits.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for DepositProduct entity
/// </summary>
public sealed class DepositProductConfiguration : IEntityTypeConfiguration<DepositProduct>
{
    public void Configure(EntityTypeBuilder<DepositProduct> builder)
    {
        builder.ToTable("deposit_products");
        builder.HasKey(dp => dp.Id);

        builder.Property(dp => dp.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(dp => dp.Description)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(dp => dp.MinimumAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(dp => dp.MaximumAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(dp => dp.MinimumTermMonths)
            .IsRequired();

        builder.Property(dp => dp.MaximumTermMonths)
            .IsRequired();

        builder.Property(dp => dp.BaseInterestRate)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(dp => dp.InterestFrequency)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(dp => dp.InterestCalculationMethod)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(dp => dp.AllowsEarlyWithdrawal)
            .IsRequired();

        builder.Property(dp => dp.WithdrawalPenaltyType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(dp => dp.WithdrawalPenaltyPercent)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(dp => dp.IsActive)
            .IsRequired();

        builder.Property(dp => dp.CreatedAtUtc)
            .IsRequired();

        builder.Property(dp => dp.LastModifiedAtUtc)
            .IsRequired();

        // Indexes
        builder.HasIndex(dp => dp.Name).IsUnique();
        builder.HasIndex(dp => dp.IsActive);
    }
}
