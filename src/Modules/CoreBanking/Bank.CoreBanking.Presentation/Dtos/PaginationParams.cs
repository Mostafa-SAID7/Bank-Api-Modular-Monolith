namespace Bank.CoreBanking.Presentation.Dtos;

/// <summary>
/// Pagination parameters for list endpoints
/// </summary>
public sealed record PaginationParams(
    int PageNumber = 1,
    int PageSize = 10
);
