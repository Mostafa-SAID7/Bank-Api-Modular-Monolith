namespace Bank.Statements.Infrastructure.Data.Configurations;

using Bank.Statements.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class StatementScheduleConfiguration : IEntityTypeConfiguration<StatementSchedule>
{
    public void Configure(EntityTypeBuilder<StatementSchedule> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(s => s.CustomerId)
            .HasColumnName("customer_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(s => s.AccountId)
            .HasColumnName("account_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(s => s.Period)
            .HasColumnName("period")
            .HasColumnType("integer")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(s => s.PreferredFormat)
            .HasColumnName("preferred_format")
            .HasColumnType("integer")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(s => s.DeliveryMethod)
            .HasColumnName("delivery_method")
            .HasColumnType("integer")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(s => s.IsActive)
            .HasColumnName("is_active")
            .HasColumnType("boolean")
            .IsRequired();

        builder.Property(s => s.NextGenerationDate)
            .HasColumnName("next_generation_date")
            .HasColumnType("timestamp without time zone");

        builder.Property(s => s.LastGeneratedDate)
            .HasColumnName("last_generated_date")
            .HasColumnType("timestamp without time zone");

        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp without time zone")
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp without time zone");

        builder.HasIndex(s => s.AccountId).HasDatabaseName("ix_statement_schedules_account_id");
        builder.HasIndex(s => s.CustomerId).HasDatabaseName("ix_statement_schedules_customer_id");
        builder.HasIndex(s => s.IsActive).HasDatabaseName("ix_statement_schedules_is_active");
    }
}
