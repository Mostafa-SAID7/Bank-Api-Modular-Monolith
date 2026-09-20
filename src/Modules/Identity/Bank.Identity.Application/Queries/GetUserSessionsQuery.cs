namespace Bank.Identity.Application.Queries;

/// <summary>
/// Query to retrieve active sessions for a user with pagination
/// </summary>
public sealed record GetUserSessionsQuery(
    Guid UserId,
    int PageNumber = 1,
    int PageSize = 10);

/// <summary>
/// Result of get user sessions query
/// </summary>
public sealed record SessionSummary(
    Guid Id,
    string IpAddress,
    string UserAgent,
    int Status,
    DateTime CreatedAtUtc,
    DateTime LastActivityAtUtc,
    DateTime ExpiresAtUtc,
    bool IsAdminSession);

/// <summary>
/// Paginated user sessions result
/// </summary>
public sealed record GetUserSessionsResult(
    IReadOnlyList<SessionSummary> Data,
    int PageNumber,
    int PageSize,
    int TotalCount)
{
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}
