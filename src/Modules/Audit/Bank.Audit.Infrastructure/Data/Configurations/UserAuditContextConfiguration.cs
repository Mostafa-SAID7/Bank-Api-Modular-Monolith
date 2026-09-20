namespace Bank.Audit.Infrastructure.Data.Configurations;

using Bank.Audit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class UserAuditContextConfiguration : IEntityTypeConfiguration<UserAuditContext>
{
    public void Configure(EntityTypeBuilder<UserAuditContext> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedNever();

        builder.Property(a => a.UserId)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(a => a.LastActivityDescription)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(a => a.UserId);
    }
}
