namespace Bank.Cards.Infrastructure.Services;

using Bank.Cards.Infrastructure.Data;
using System.Text;

/// <summary>
/// Service for generating unique card numbers
/// Format: uses Luhn algorithm for valid card numbers
/// In production, this would integrate with actual card provider
/// </summary>
public sealed class CardNumberGenerator : ICardNumberGenerator
{
    private readonly CardsDbContext _context;

    public CardNumberGenerator(CardsDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateAsync(CancellationToken cancellationToken = default)
    {
        string cardNumber;
        bool isUnique = false;

        do
        {
            // Generate a random 16-digit card number (simplified for demo)
            // In production, would use actual card provider API or BIN prefixes
            cardNumber = GenerateRandomCardNumber();
            
            // Check if this card number already exists
            var exists = await _context.Cards
                .AnyAsync(c => c.CardNumber == cardNumber, cancellationToken);
            
            isUnique = !exists;
        } while (!isUnique);

        return cardNumber;
    }

    private static string GenerateRandomCardNumber()
    {
        // Generate 15 random digits
        var random = Random.Shared;
        var digits = new StringBuilder();
        
        // Start with Visa prefix (4) or MasterCard prefix (5)
        var prefix = random.Next(0, 2) == 0 ? "4" : "5";
        digits.Append(prefix);

        // Add 15 more random digits
        for (int i = 0; i < 15; i++)
        {
            digits.Append(random.Next(0, 10));
        }

        // Calculate Luhn check digit
        var cardNumberWithoutCheck = digits.ToString();
        var checkDigit = CalculateLuhnCheckDigit(cardNumberWithoutCheck);
        
        return cardNumberWithoutCheck + checkDigit;
    }

    private static int CalculateLuhnCheckDigit(string cardNumber)
    {
        int sum = 0;
        bool isEvenPosition = false;

        // Process digits from right to left
        for (int i = cardNumber.Length - 1; i >= 0; i--)
        {
            int digit = int.Parse(cardNumber[i].ToString());

            if (isEvenPosition)
            {
                digit *= 2;
                if (digit > 9)
                {
                    digit -= 9;
                }
            }

            sum += digit;
            isEvenPosition = !isEvenPosition;
        }

        // Return check digit
        return (10 - (sum % 10)) % 10;
    }
}
