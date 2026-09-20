namespace Bank.Deposits.Presentation.Dtos;

/// <summary>
/// Error response DTO for API error responses
/// </summary>
public record ErrorResponse(
    string Message,
    string? Details = null,
    string? Code = null);
