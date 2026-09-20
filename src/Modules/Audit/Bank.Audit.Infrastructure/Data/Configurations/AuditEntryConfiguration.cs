namespace Bank.Audit.Infrastructure.Data.Configurations;

using Bank.Audit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class AuditEntryConfiguration : IEntityTypeConfiguration<AuditEntry>
{
    public void Configure(EntityTypeBuilder<AuditEntry> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedNever();

        builder.Property(a => a.EntityName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(a => a.OldValues)
            .HasMaxLength(5000);

        builder.Property(a => a.NewValues)
            .HasMaxLength(5000);

        builder.Property(a => a.ActionType)
            .HasConversion<int>();

        builder.Property(a => a.ChangeType)
            .HasConversion<int>();

        builder.HasIndex(a => a.AuditLogId);
    }
}
