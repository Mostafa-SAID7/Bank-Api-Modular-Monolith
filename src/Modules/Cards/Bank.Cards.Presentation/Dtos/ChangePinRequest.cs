namespace Bank.Cards.Presentation.Dtos;

/// <summary>
/// Request DTO for changing card PIN
/// </summary>
public record ChangePinRequest(
    Guid CardId,
    string CurrentPin,
    string NewPin);
