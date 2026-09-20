namespace Bank.Cards.Presentation.Dtos;

/// <summary>
/// Request DTO for issuing a new card
/// </summary>
public record IssueCardRequest(
    Guid CustomerId,
    Guid LinkedAccountId,
    Guid CardProductId,
    string HolderName);
