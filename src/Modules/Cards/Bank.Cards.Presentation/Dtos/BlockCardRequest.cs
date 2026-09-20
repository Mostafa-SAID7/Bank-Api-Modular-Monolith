namespace Bank.Cards.Presentation.Dtos;

/// <summary>
/// Request DTO for blocking a card
/// </summary>
public record BlockCardRequest(
    Guid CardId,
    BlockReason Reason,
    string Description = "");
