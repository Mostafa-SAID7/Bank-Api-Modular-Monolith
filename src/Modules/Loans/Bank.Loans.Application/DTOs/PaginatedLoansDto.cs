namespace Bank.Loans.Application.DTOs;

public record PaginatedLoansDto(
    List<LoanDetailDto> Items,
    int TotalCount,
    int PageNumber,
    int PageSize);
