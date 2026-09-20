namespace Bank.Cards.Application.Interfaces;

/// <summary>
/// Service to encrypt/decrypt sensitive card data
/// </summary>
public interface ICardEncryptionService
{
    string EncryptCardNumber(string cardNumber);
    string DecryptCardNumber(string encryptedCardNumber);
    string HashPin(string pin);
    bool VerifyPin(string pin, string hash);
}
