namespace Bank.Audit.Infrastructure.Data.Configurations;

using Bank.Audit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedNever();

        builder.Property(a => a.UserId)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(a => a.ResourceId)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(a => a.Action)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(a => a.IpAddress)
            .IsRequired()
            .HasMaxLength(45);

        builder.Property(a => a.UserAgent)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.Details)
            .HasMaxLength(2000);

        builder.Property(a => a.EventType)
            .HasConversion<int>();

        builder.Property(a => a.ResourceType)
            .HasConversion<int>();

        builder.Property(a => a.Status)
            .HasConversion<int>();

        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.Timestamp);
        builder.HasIndex(a => a.Status);
    }
}
