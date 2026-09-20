namespace Bank.Deposits.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for FixedDeposit entity
/// </summary>
public sealed class FixedDepositConfiguration : IEntityTypeConfiguration<FixedDeposit>
{
    public void Configure(EntityTypeBuilder<FixedDeposit> builder)
    {
        builder.ToTable("fixed_deposits");
        builder.HasKey(fd => fd.Id);

        builder.Property(fd => fd.DepositNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(fd => fd.CustomerId)
            .IsRequired();

        builder.Property(fd => fd.LinkedAccountId)
            .IsRequired();

        builder.Property(fd => fd.DepositProductId)
            .IsRequired();

        builder.Property(fd => fd.PrincipalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(fd => fd.InterestRate)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(fd => fd.TermDays)
            .IsRequired();

        builder.Property(fd => fd.StartDateUtc)
            .IsRequired();

        builder.Property(fd => fd.MaturityDateUtc)
            .IsRequired();

        builder.Property(fd => fd.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(fd => fd.InterestCalculationMethod)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(fd => fd.CompoundingFrequency)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(fd => fd.AccruedInterest)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(fd => fd.LastInterestCalculationDateUtc)
            .IsRequired();

        builder.Property(fd => fd.MaturityAction)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(fd => fd.AutoRenewalEnabled)
            .IsRequired();

        builder.Property(fd => fd.RenewalTermDays)
            .IsRequired(false);

        builder.Property(fd => fd.RenewalNoticeDateUtc)
            .IsRequired(false);

        builder.Property(fd => fd.CustomerConsentReceived)
            .IsRequired();

        builder.Property(fd => fd.PenaltyType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(fd => fd.PenaltyAmount)
            .HasPrecision(18, 2)
            .IsRequired(false);

        builder.Property(fd => fd.PenaltyPercentage)
            .HasPrecision(5, 2)
            .IsRequired(false);

        builder.Property(fd => fd.ClosureDateUtc)
            .IsRequired(false);

        builder.Property(fd => fd.ClosedByUserId)
            .IsRequired(false);

        builder.Property(fd => fd.ClosureReason)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(fd => fd.PenaltyApplied)
            .HasPrecision(18, 2)
            .IsRequired(false);

        builder.Property(fd => fd.NetAmountPaid)
            .HasPrecision(18, 2)
            .IsRequired(false);

        builder.Property(fd => fd.RenewedFromDepositId)
            .IsRequired(false);

        builder.Property(fd => fd.RenewedToDepositId)
            .IsRequired(false);

        builder.Property(fd => fd.RenewalCount)
            .IsRequired();

        builder.Property(fd => fd.CreatedAtUtc)
            .IsRequired();

        builder.Property(fd => fd.UpdatedAtUtc)
            .IsRequired();

        // Indexes
        builder.HasIndex(fd => fd.DepositNumber).IsUnique();
        builder.HasIndex(fd => fd.CustomerId);
        builder.HasIndex(fd => fd.Status);
        builder.HasIndex(fd => fd.MaturityDateUtc);
        builder.HasIndex(fd => fd.CreatedAtUtc);
    }
}
