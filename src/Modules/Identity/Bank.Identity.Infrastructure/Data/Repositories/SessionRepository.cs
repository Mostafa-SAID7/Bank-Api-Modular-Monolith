using Bank.Identity.Application.Interfaces;
using Bank.Identity.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Bank.Identity.Infrastructure.Data.Repositories;

/// <summary>
/// EF Core implementation of ISessionRepository
/// </summary>
public sealed class SessionRepository : ISessionRepository
{
    private readonly IdentityDbContext _dbContext;

    public SessionRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Session session, CancellationToken cancellationToken = default)
    {
        await _dbContext.Sessions.AddAsync(session, cancellationToken);
    }

    public async Task UpdateAsync(Session session, CancellationToken cancellationToken = default)
    {
        _dbContext.Sessions.Update(session);
        await Task.CompletedTask;
    }

    public async Task<Session?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Session?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions
            .FirstOrDefaultAsync(s => s.SessionToken == token, cancellationToken);
    }

    public async Task<IReadOnlyList<Session>> GetUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions
            .Where(s => s.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Session>> GetActiveSessionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions
            .Where(s => s.UserId == userId && s.Status == SessionStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var session = await _dbContext.Sessions.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        if (session != null)
        {
            _dbContext.Sessions.Remove(session);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
