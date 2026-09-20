namespace Bank.Cards.Infrastructure.Services;

/// <summary>
/// Service for encrypting/decrypting sensitive card data and hashing PINs
/// In production, should use Azure Key Vault or similar for key management
/// </summary>
public sealed class CardEncryptionService : ICardEncryptionService
{
    public string EncryptCardNumber(string cardNumber)
    {
        // In production, use actual encryption (AES, etc.)
        // For now, just base64 encode as placeholder
        var bytes = System.Text.Encoding.UTF8.GetBytes(cardNumber);
        return Convert.ToBase64String(bytes);
    }

    public string DecryptCardNumber(string encryptedCardNumber)
    {
        try
        {
            // In production, use actual decryption
            var bytes = Convert.FromBase64String(encryptedCardNumber);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            throw new InvalidOperationException("Failed to decrypt card number");
        }
    }

    public string HashPin(string pin)
    {
        // In production, use BCrypt.Net-Next or Argon2
        // For now, use simple SHA256 hashing (NOT SECURE FOR PRODUCTION)
        using var sha = System.Security.Cryptography.SHA256.Create();
        var hash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(pin));
        return Convert.ToBase64String(hash);
    }

    public bool VerifyPin(string pin, string hash)
    {
        try
        {
            var pinHash = HashPin(pin);
            return pinHash == hash;
        }
        catch
        {
            return false;
        }
    }
}
