using Bank.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bank.Identity.Infrastructure.Data;

/// <summary>
/// EF Core DbContext for Identity module
/// Schema: identity in PostgreSQL
/// Extends IdentityDbContext for ASP.NET Core Identity support
/// </summary>
public sealed class IdentityDbContext : IdentityDbContext<User, Role, Guid>
{
    public DbSet<Session> Sessions => Set<Session>();

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Set schema
        modelBuilder.HasDefaultSchema("identity");

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.LastName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.Status).HasConversion<int>();
            entity.Property(u => u.CreatedAtUtc).IsRequired();
            entity.Property(u => u.UpdatedAtUtc);
            entity.Property(u => u.DeletedAtUtc);
            entity.Property(u => u.IsDeleted).HasDefaultValue(false);
            entity.Property(u => u.FailedLoginAttempts).HasDefaultValue(0);
            entity.Property(u => u.LastLoginAtUtc);
            entity.Property(u => u.LockoutStartAtUtc);

            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.IsDeleted);
            entity.HasIndex(u => u.Status);
        });

        // Role configuration
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");
            entity.Property(r => r.Description).HasMaxLength(500);
            entity.Property(r => r.CreatedAtUtc).IsRequired();
            entity.Property(r => r.UpdatedAtUtc);
        });

        // Session configuration
        modelBuilder.Entity<Session>(entity =>
        {
            entity.ToTable("sessions");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.UserId).IsRequired();
            entity.Property(s => s.SessionToken).HasMaxLength(1000).IsRequired();
            entity.Property(s => s.RefreshTokenHash).HasMaxLength(1000);
            entity.Property(s => s.ReplacedByToken).HasMaxLength(1000);
            entity.Property(s => s.Status).HasConversion<int>();
            entity.Property(s => s.IpAddress).HasMaxLength(50).IsRequired();
            entity.Property(s => s.UserAgent).HasMaxLength(1000).IsRequired();
            entity.Property(s => s.DeviceFingerprint).HasMaxLength(500);
            entity.Property(s => s.TerminationReason).HasMaxLength(500);
            entity.Property(s => s.IsAdminSession).HasDefaultValue(false);

            // Foreign key
            entity.HasOne(s => s.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(s => s.UserId);
            entity.HasIndex(s => s.SessionToken).IsUnique();
            entity.HasIndex(s => s.Status);
            entity.HasIndex(s => s.CreatedAtUtc);
        });

        // IdentityUserClaim configuration
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<Guid>>(entity =>
        {
            entity.ToTable("user_claims");
        });

        // IdentityUserLogin configuration
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<Guid>>(entity =>
        {
            entity.ToTable("user_logins");
        });

        // IdentityUserToken configuration
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<Guid>>(entity =>
        {
            entity.ToTable("user_tokens");
        });

        // IdentityRoleClaim configuration
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<Guid>>(entity =>
        {
            entity.ToTable("role_claims");
        });

        // IdentityUserRole configuration (junction table)
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<Guid>>(entity =>
        {
            entity.ToTable("user_roles");
        });
    }
}
