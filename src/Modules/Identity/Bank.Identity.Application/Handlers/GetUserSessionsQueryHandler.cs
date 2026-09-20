using Bank.Identity.Application.Queries;

namespace Bank.Identity.Application.Handlers;

/// <summary>
/// Handler for GetUserSessionsQuery
/// </summary>
public sealed class GetUserSessionsQueryHandler
{
    private readonly ISessionRepository _sessionRepository;

    public GetUserSessionsQueryHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<GetUserSessionsResult> Handle(
        GetUserSessionsQuery query,
        CancellationToken cancellationToken)
    {
        var sessions = await _sessionRepository.GetUserSessionsAsync(query.UserId, cancellationToken);

        var totalCount = sessions.Count;
        var pagedSessions = sessions
            .OrderByDescending(s => s.CreatedAtUtc)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(s => new SessionSummary(
                s.Id,
                s.IpAddress,
                s.UserAgent,
                (int)s.Status,
                s.CreatedAtUtc,
                s.LastActivityAtUtc,
                s.ExpiresAtUtc,
                s.IsAdminSession
            ))
            .ToList();

        return new GetUserSessionsResult(
            pagedSessions,
            query.PageNumber,
            query.PageSize,
            totalCount
        );
    }
}
