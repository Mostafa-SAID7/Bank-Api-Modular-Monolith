namespace Bank.Cards.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for CardTransaction entity
/// </summary>
public sealed class CardTransactionConfiguration : IEntityTypeConfiguration<CardTransaction>
{
    public void Configure(EntityTypeBuilder<CardTransaction> builder)
    {
        builder.ToTable("card_transactions");
        builder.HasKey(ct => ct.Id);

        builder.Property(ct => ct.CardId)
            .IsRequired();

        builder.Property(ct => ct.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(ct => ct.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(ct => ct.FeeAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(ct => ct.NetAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(ct => ct.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(ct => ct.DeclineReason)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(ct => ct.Merchant)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(ct => ct.MerchantCategory)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(ct => ct.IsInternational)
            .IsRequired();

        builder.Property(ct => ct.IsContactless)
            .IsRequired();

        builder.Property(ct => ct.AuthorizationCode)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(ct => ct.ReferenceNumber)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(ct => ct.TransactionDateUtc)
            .IsRequired();

        builder.Property(ct => ct.PostingDateUtc)
            .IsRequired(false);

        builder.Property(ct => ct.CreatedAtUtc)
            .IsRequired();

        builder.Property(ct => ct.IsSuspicious)
            .IsRequired();

        builder.Property(ct => ct.SuspiciousReason)
            .HasMaxLength(500)
            .IsRequired(false);

        // Indexes
        builder.HasIndex(ct => ct.CardId);
        builder.HasIndex(ct => ct.Status);
        builder.HasIndex(ct => ct.TransactionDateUtc);
        builder.HasIndex(ct => ct.CreatedAtUtc);
    }
}
