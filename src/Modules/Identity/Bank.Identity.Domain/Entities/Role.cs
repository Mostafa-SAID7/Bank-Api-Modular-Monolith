using Microsoft.AspNetCore.Identity;

namespace Bank.Identity.Domain.Entities;

/// <summary>
/// Role aggregate root - extends ASP.NET Core Identity
/// Migrated from Bank.Domain to Identity module
/// Uses Guid as primary key
/// </summary>
public class Role : IdentityRole<Guid>
{
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Factory method for creating a new role
    /// </summary>
    public static Role Create(
        string name,
        string? description = null,
        string? createdBy = null)
    {
        return new Role
        {
            Id = Guid.NewGuid(),
            Name = name,
            NormalizedName = name.ToUpperInvariant(),
            Description = description,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
}
