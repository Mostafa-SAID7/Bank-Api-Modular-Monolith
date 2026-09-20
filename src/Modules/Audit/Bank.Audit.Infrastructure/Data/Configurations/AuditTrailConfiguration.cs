namespace Bank.Audit.Infrastructure.Data.Configurations;

using Bank.Audit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class AuditTrailConfiguration : IEntityTypeConfiguration<AuditTrail>
{
    public void Configure(EntityTypeBuilder<AuditTrail> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedNever();

        builder.Property(a => a.TrailName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(a => a.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.FileFormat)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(a => a.GeneratedAt);
    }
}
