namespace Bank.Cards.Presentation.Dtos;

/// <summary>
/// Request DTO for activating a card
/// </summary>
public record ActivateCardRequest(
    Guid CardId,
    string Pin);
