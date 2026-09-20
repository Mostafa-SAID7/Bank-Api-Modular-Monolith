namespace Bank.Deposits.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for DepositTransaction entity
/// </summary>
public sealed class DepositTransactionConfiguration : IEntityTypeConfiguration<DepositTransaction>
{
    public void Configure(EntityTypeBuilder<DepositTransaction> builder)
    {
        builder.ToTable("deposit_transactions");
        builder.HasKey(dt => dt.Id);

        builder.Property(dt => dt.FixedDepositId)
            .IsRequired();

        builder.Property(dt => dt.TransactionType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(dt => dt.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(dt => dt.Penalty)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(dt => dt.NetAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(dt => dt.TransactionDateUtc)
            .IsRequired();

        builder.Property(dt => dt.CreatedAtUtc)
            .IsRequired();

        // Indexes
        builder.HasIndex(dt => dt.FixedDepositId);
        builder.HasIndex(dt => dt.TransactionType);
        builder.HasIndex(dt => dt.TransactionDateUtc);
    }
}
