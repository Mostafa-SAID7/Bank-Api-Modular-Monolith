namespace Bank.Deposits.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for MaturityNotice entity
/// </summary>
public sealed class MaturityNoticeConfiguration : IEntityTypeConfiguration<MaturityNotice>
{
    public void Configure(EntityTypeBuilder<MaturityNotice> builder)
    {
        builder.ToTable("maturity_notices");
        builder.HasKey(mn => mn.Id);

        builder.Property(mn => mn.FixedDepositId)
            .IsRequired();

        builder.Property(mn => mn.CustomerId)
            .IsRequired();

        builder.Property(mn => mn.FixedDepositNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(mn => mn.MaturityDate)
            .IsRequired();

        builder.Property(mn => mn.DueDateUtc)
            .IsRequired();

        builder.Property(mn => mn.NotificationSent)
            .IsRequired();

        builder.Property(mn => mn.NotificationSentAtUtc)
            .IsRequired(false);

        builder.Property(mn => mn.CustomerChoiceProvided)
            .IsRequired();

        builder.Property(mn => mn.CustomerChoiceProvidedAtUtc)
            .IsRequired(false);

        builder.Property(mn => mn.CustomerChoice)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(mn => mn.CreatedAtUtc)
            .IsRequired();

        builder.Property(mn => mn.LastModifiedAtUtc)
            .IsRequired();

        // Indexes
        builder.HasIndex(mn => mn.FixedDepositId).IsUnique();
        builder.HasIndex(mn => mn.CustomerId);
        builder.HasIndex(mn => mn.NotificationSent);
        builder.HasIndex(mn => mn.MaturityDate);
    }
}
