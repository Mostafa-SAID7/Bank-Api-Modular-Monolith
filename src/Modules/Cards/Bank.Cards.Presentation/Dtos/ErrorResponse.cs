namespace Bank.Cards.Presentation.Dtos;

/// <summary>
/// Standard error response DTO
/// </summary>
public record ErrorResponse(
    string Message,
    int StatusCode,
    string? Details = null);
