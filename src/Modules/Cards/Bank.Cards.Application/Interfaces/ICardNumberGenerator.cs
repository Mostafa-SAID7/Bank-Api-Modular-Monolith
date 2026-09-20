namespace Bank.Cards.Application.Interfaces;

/// <summary>
/// Service to generate unique card numbers
/// </summary>
public interface ICardNumberGenerator
{
    Task<string> GenerateAsync(CancellationToken cancellationToken = default);
}
