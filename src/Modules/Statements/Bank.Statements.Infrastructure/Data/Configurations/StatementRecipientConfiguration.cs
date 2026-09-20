namespace Bank.Statements.Infrastructure.Data.Configurations;

using Bank.Statements.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class StatementRecipientConfiguration : IEntityTypeConfiguration<StatementRecipient>
{
    public void Configure(EntityTypeBuilder<StatementRecipient> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(r => r.StatementId)
            .HasColumnName("statement_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(r => r.Email)
            .HasColumnName("email")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(r => r.DeliveryMethod)
            .HasColumnName("delivery_method")
            .HasColumnType("integer")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(r => r.HasReceived)
            .HasColumnName("has_received")
            .HasColumnType("boolean")
            .IsRequired();

        builder.Property(r => r.ReceivedAt)
            .HasColumnName("received_at")
            .HasColumnType("timestamp without time zone");

        builder.Property(r => r.DeliveryReference)
            .HasColumnName("delivery_reference")
            .HasColumnType("text");

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp without time zone")
            .IsRequired();

        builder.HasIndex(r => r.StatementId).HasDatabaseName("ix_statement_recipients_statement_id");
        builder.HasIndex(r => r.Email).HasDatabaseName("ix_statement_recipients_email");
    }
}
