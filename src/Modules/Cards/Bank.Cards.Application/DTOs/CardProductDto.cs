namespace Bank.Cards.Application.DTOs;

/// <summary>
/// DTO for CardProduct entity
/// </summary>
public sealed record CardProductDto(
    Guid Id,
    string ProductName,
    CardType CardType,
    CardBrand Brand,
    decimal DefaultDailyLimit,
    decimal DefaultTransactionLimit,
    bool RequiresPinForAtm,
    int CardValidityYears,
    bool IsActive);
