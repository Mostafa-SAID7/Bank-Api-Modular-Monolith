namespace Bank.Identity.Presentation.Dtos;

/// <summary>
/// Error response DTO
/// </summary>
public sealed record ErrorResponse(
    string Message,
    string? Code = null,
    int? StatusCode = null);
