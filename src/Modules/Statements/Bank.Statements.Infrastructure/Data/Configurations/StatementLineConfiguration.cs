namespace Bank.Statements.Infrastructure.Data.Configurations;

using Bank.Statements.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class StatementLineConfiguration : IEntityTypeConfiguration<StatementLine>
{
    public void Configure(EntityTypeBuilder<StatementLine> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(l => l.StatementId)
            .HasColumnName("statement_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(l => l.TransactionDate)
            .HasColumnName("transaction_date")
            .HasColumnType("timestamp without time zone")
            .IsRequired();

        builder.Property(l => l.PostedDate)
            .HasColumnName("posted_date")
            .HasColumnType("timestamp without time zone")
            .IsRequired();

        builder.Property(l => l.Description)
            .HasColumnName("description")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(l => l.LineType)
            .HasColumnName("line_type")
            .HasColumnType("integer")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(l => l.Amount)
            .HasColumnName("amount")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(l => l.Balance)
            .HasColumnName("balance")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(l => l.Reference)
            .HasColumnName("reference")
            .HasColumnType("text");

        builder.Property(l => l.ChequeNumber)
            .HasColumnName("cheque_number")
            .HasColumnType("text");

        builder.Property(l => l.IsPosted)
            .HasColumnName("is_posted")
            .HasColumnType("boolean")
            .IsRequired();

        builder.Property(l => l.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp without time zone")
            .IsRequired();

        builder.HasIndex(l => l.StatementId).HasDatabaseName("ix_statement_lines_statement_id");
        builder.HasIndex(l => l.TransactionDate).HasDatabaseName("ix_statement_lines_transaction_date");
    }
}
