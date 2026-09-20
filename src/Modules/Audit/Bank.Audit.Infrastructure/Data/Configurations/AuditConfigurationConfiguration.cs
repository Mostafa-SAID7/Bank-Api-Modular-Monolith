namespace Bank.Audit.Infrastructure.Data.Configurations;

using Bank.Audit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class AuditConfigurationConfiguration : IEntityTypeConfiguration<AuditConfiguration>
{
    public void Configure(EntityTypeBuilder<AuditConfiguration> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedNever();

        builder.Property(a => a.AuditLevel)
            .HasConversion<int>();

        builder.Property(a => a.NotificationEmail)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(a => a.ModifiedBy)
            .IsRequired()
            .HasMaxLength(256);
    }
}
