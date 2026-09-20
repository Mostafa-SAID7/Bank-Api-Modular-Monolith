namespace Bank.Identity.Application.Interfaces;

/// <summary>
/// Repository interface for Session aggregate
/// Owned by Identity module
/// </summary>
public interface ISessionRepository
{
    Task AddAsync(Session session, CancellationToken cancellationToken = default);
    Task UpdateAsync(Session session, CancellationToken cancellationToken = default);
    Task<Session?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Session?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Session>> GetUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Session>> GetActiveSessionsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
