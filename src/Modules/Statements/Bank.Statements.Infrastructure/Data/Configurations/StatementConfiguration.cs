namespace Bank.Statements.Infrastructure.Data.Configurations;

using Bank.Statements.Domain.Entities;
using Bank.Statements.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class StatementConfiguration : IEntityTypeConfiguration<Statement>
{
    public void Configure(EntityTypeBuilder<Statement> builder)
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

        builder.Property(s => s.StatementDate)
            .HasColumnName("statement_date")
            .HasColumnType("timestamp without time zone")
            .IsRequired();

        builder.Property(s => s.PeriodStartDate)
            .HasColumnName("period_start_date")
            .HasColumnType("timestamp without time zone")
            .IsRequired();

        builder.Property(s => s.PeriodEndDate)
            .HasColumnName("period_end_date")
            .HasColumnType("timestamp without time zone")
            .IsRequired();

        builder.Property(s => s.OpeningBalance)
            .HasColumnName("opening_balance")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(s => s.ClosingBalance)
            .HasColumnName("closing_balance")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(s => s.TotalDebits)
            .HasColumnName("total_debits")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(s => s.TotalCredits)
            .HasColumnName("total_credits")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(s => s.Status)
            .HasColumnName("status")
            .HasColumnType("integer")
            .HasConversion<int>()
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

        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp without time zone")
            .IsRequired();

        builder.Property(s => s.GeneratedAt)
            .HasColumnName("generated_at")
            .HasColumnType("timestamp without time zone");

        builder.Property(s => s.SentAt)
            .HasColumnName("sent_at")
            .HasColumnType("timestamp without time zone");

        builder.Property(s => s.DownloadedAt)
            .HasColumnName("downloaded_at")
            .HasColumnType("timestamp without time zone");

        builder.Property(s => s.ArchivedAt)
            .HasColumnName("archived_at")
            .HasColumnType("timestamp without time zone");

        builder.Property(s => s.ArchiveReason)
            .HasColumnName("archive_reason")
            .HasColumnType("integer")
            .HasConversion<int?>();

        builder.Property(s => s.ArchiveNotes)
            .HasColumnName("archive_notes")
            .HasColumnType("text");

        builder.Property(s => s.ExportStatus)
            .HasColumnName("export_status")
            .HasColumnType("integer")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(s => s.ExportReference)
            .HasColumnName("export_reference")
            .HasColumnType("text");

        builder.Property(s => s.Visibility)
            .HasColumnName("visibility")
            .HasColumnType("integer")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(s => s.IsAccessible)
            .HasColumnName("is_accessible")
            .HasColumnType("boolean")
            .IsRequired();

        builder.HasIndex(s => s.CustomerId).HasDatabaseName("ix_statements_customer_id");
        builder.HasIndex(s => s.AccountId).HasDatabaseName("ix_statements_account_id");
        builder.HasIndex(s => s.Status).HasDatabaseName("ix_statements_status");
    }
}
