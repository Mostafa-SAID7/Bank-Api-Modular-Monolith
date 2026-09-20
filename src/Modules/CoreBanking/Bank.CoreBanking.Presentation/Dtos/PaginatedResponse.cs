namespace Bank.CoreBanking.Presentation.Dtos;

/// <summary>
/// Generic paginated response wrapper
/// </summary>
/// <typeparam name="T">Type of items in the page</typeparam>
public sealed record PaginatedResponse<T>(
    IReadOnlyList<T> Data,
    int PageNumber,
    int PageSize,
    int TotalCount)
{
    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;

    /// <summary>
    /// Whether there are more pages after this one
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Whether there are pages before this one
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;
}
