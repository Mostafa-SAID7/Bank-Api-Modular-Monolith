namespace Bank.Deposits.Presentation.Dtos;

/// <summary>
/// Pagination parameters for API requests
/// </summary>
public record PaginationParams(
    int PageNumber = 1,
    int PageSize = 10);
