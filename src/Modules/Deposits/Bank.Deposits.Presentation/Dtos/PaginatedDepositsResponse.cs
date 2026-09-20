namespace Bank.Deposits.Presentation.Dtos;

/// <summary>
/// Response DTO for paginated customer deposits
/// </summary>
public record PaginatedDepositsResponse(
    List<DepositResponse> Items,
    int TotalCount,
    int PageNumber,
    int PageSize);
