namespace Bank.Cards.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for CardBlock entity
/// </summary>
public sealed class CardBlockConfiguration : IEntityTypeConfiguration<CardBlock>
{
    public void Configure(EntityTypeBuilder<CardBlock> builder)
    {
        builder.ToTable("card_blocks");
        builder.HasKey(cb => cb.Id);

        builder.Property(cb => cb.CardId)
            .IsRequired();

        builder.Property(cb => cb.Reason)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(cb => cb.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(cb => cb.BlockedAtUtc)
            .IsRequired();

        builder.Property(cb => cb.UnblockedAtUtc)
            .IsRequired(false);

        // Indexes
        builder.HasIndex(cb => cb.CardId);
        builder.HasIndex(cb => cb.BlockedAtUtc);
    }
}
