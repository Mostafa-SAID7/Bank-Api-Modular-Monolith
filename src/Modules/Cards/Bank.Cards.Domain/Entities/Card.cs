using System.Security.Cryptography;
using System.Text;

namespace Bank.Cards.Domain.Entities;

/// <summary>
/// Represents a payment card (debit, credit, or prepaid)
/// Aggregate root for card lifecycle management
/// </summary>
public class Card
{
    /// <summary>Aggregate root Id</summary>
    public Guid Id { get; private set; }

    public string CardNumber { get; private set; } = string.Empty;
    public string MaskedPan { get; private set; } = string.Empty; // 4539-****-****-1234
    public Guid CustomerId { get; private set; }
    public Guid LinkedAccountId { get; private set; }
    public Guid CardProductId { get; private set; }
    
    // Card details
    public CardType CardType { get; private set; }
    public CardBrand CardBrand { get; private set; }
    public CardStatus Status { get; private set; }
    public string HolderName { get; private set; } = string.Empty;
    public string CvvHash { get; private set; } = string.Empty; // Hashed CVV (Bcrypt/Argon2)
    
    // Lifecycle
    public DateTime IssuedDateUtc { get; private set; }
    public DateTime ExpiryDateUtc { get; private set; }
    public DateTime? ActivatedDateUtc { get; private set; }
    public DateTime? BlockedDateUtc { get; private set; }
    public DateTime? ClosedDateUtc { get; private set; }
    
    // Limits and controls
    public decimal DailyWithdrawalLimit { get; private set; }
    public decimal DailyTransactionLimit { get; private set; }
    public decimal DailyForeignTransactionLimit { get; private set; }
    public int DailyTransactionCount { get; private set; }
    public bool InternationalEnabled { get; private set; }
    public bool OnlineTransactionsEnabled { get; private set; }
    public bool ContactlessEnabled { get; private set; }
    
    // PIN security
    public string? PinHash { get; private set; } // Bcrypt/Argon2 hashed
    public int FailedPinAttempts { get; private set; }
    public DateTime? LastPinChangeUtc { get; private set; }
    public bool PinRequired { get; private set; }
    public DateTime? PinLockedUntilUtc { get; private set; } // Lock after 3 failed attempts
    
    // Replacement tracking
    public Guid? ReplacedFromCardId { get; private set; }
    public Guid? ReplacedToCardId { get; private set; }
    public int ReplacementCount { get; private set; }
    
    // Current period tracking (for limits)
    public DateTime LimitResetDateUtc { get; private set; }
    public decimal CurrentDayWithdrawalAmount { get; private set; }
    public decimal CurrentDayTransactionAmount { get; private set; }
    
    // Audit
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    // Domain events
    private readonly List<object> _domainEvents = new();
    public IReadOnlyList<object> DomainEvents => _domainEvents.AsReadOnly();

    private void RaiseDomainEvent(object domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Create a new card
    /// </summary>
    public static Card Issue(
        Guid customerId,
        Guid linkedAccountId,
        Guid productId,
        CardProduct product,
        string holderName,
        string cardNumber,
        string cvv)
    {
        var card = new Card
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            LinkedAccountId = linkedAccountId,
            CardProductId = productId,
            CardType = product.CardType,
            CardBrand = product.Brand,
            HolderName = holderName,
            CardNumber = cardNumber,
            MaskedPan = MaskPan(cardNumber),
            CvvHash = HashCvv(cvv),
            
            Status = CardStatus.Inactive,
            IssuedDateUtc = DateTime.UtcNow,
            ExpiryDateUtc = DateTime.UtcNow.AddYears(product.CardValidityYears),
            
            DailyWithdrawalLimit = product.DefaultDailyLimit,
            DailyTransactionLimit = product.DefaultTransactionLimit,
            DailyForeignTransactionLimit = product.DefaultDailyLimit * 0.8m, // 80% of daily limit
            InternationalEnabled = true,
            OnlineTransactionsEnabled = true,
            ContactlessEnabled = true,
            
            PinRequired = product.RequiresPinForAtm,
            FailedPinAttempts = 0,
            ReplacementCount = 0,
            
            LimitResetDateUtc = DateTime.UtcNow,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        };

        card.RaiseDomainEvent(new CardIssuedDomainEvent(
            card.Id,
            customerId,
            card.MaskedPan,
            holderName,
            cardNumber));

        return card;
    }

    /// <summary>
    /// Activate the card (usually requires PIN entry)
    /// </summary>
    public void Activate()
    {
        if (Status != CardStatus.Inactive)
            throw new InvalidOperationException($"Card cannot be activated from {Status} status");

        Status = CardStatus.Active;
        ActivatedDateUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;

        RaiseDomainEvent(new CardActivatedDomainEvent(Id, CustomerId));
    }

    /// <summary>
    /// Block the card
    /// </summary>
    public void Block(BlockReason reason, string description = "")
    {
        if (Status == CardStatus.Closed)
            throw new InvalidOperationException("Cannot block a closed card");

        Status = CardStatus.Blocked;
        BlockedDateUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;

        RaiseDomainEvent(new CardBlockedDomainEvent(Id, CustomerId, reason, description));
    }

    /// <summary>
    /// Unblock a previously blocked card
    /// </summary>
    public void Unblock()
    {
        if (Status != CardStatus.Blocked)
            throw new InvalidOperationException("Only blocked cards can be unblocked");

        Status = CardStatus.Active;
        BlockedDateUtc = null;
        UpdatedAtUtc = DateTime.UtcNow;

        RaiseDomainEvent(new CardUnblockedDomainEvent(Id, CustomerId));
    }

    /// <summary>
    /// Close the card permanently
    /// </summary>
    public void Close(string reason = "")
    {
        if (Status == CardStatus.Closed)
            throw new InvalidOperationException("Card is already closed");

        Status = CardStatus.Closed;
        ClosedDateUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;

        RaiseDomainEvent(new CardClosedDomainEvent(Id, CustomerId, reason));
    }

    /// <summary>
    /// Replace the card (due to expiry, damage, etc.)
    /// </summary>
    public Card Replace(CardProduct product, string newCardNumber, string newCvv)
    {
        if (Status == CardStatus.Closed)
            throw new InvalidOperationException("Cannot replace a closed card");

        var replacementCard = Issue(
            CustomerId,
            LinkedAccountId,
            CardProductId,
            product,
            HolderName,
            newCardNumber,
            newCvv);

        replacementCard.ReplacedFromCardId = Id;
        ReplacedToCardId = replacementCard.Id;
        ReplacementCount++;
        UpdatedAtUtc = DateTime.UtcNow;

        RaiseDomainEvent(new CardReplacedDomainEvent(
            Id,
            CustomerId,
            replacementCard.Id,
            replacementCard.MaskedPan));

        return replacementCard;
    }

    /// <summary>
    /// Validate PIN against stored hash
    /// </summary>
    public void ValidatePin(string pin)
    {
        if (Status != CardStatus.Active)
            throw new InvalidOperationException("Cannot validate PIN on inactive card");

        if (!PinRequired)
            return; // PIN not required

        if (PinLockedUntilUtc.HasValue && DateTime.UtcNow < PinLockedUntilUtc)
            throw new InvalidOperationException("Card PIN is locked due to multiple failed attempts");

        if (string.IsNullOrEmpty(PinHash))
            throw new InvalidOperationException("No PIN set for this card");

        // In real implementation, use BCrypt.Net or similar
        // For now, simplified comparison
        if (!VerifyPin(pin, PinHash))
        {
            FailedPinAttempts++;
            if (FailedPinAttempts >= 3)
            {
                PinLockedUntilUtc = DateTime.UtcNow.AddMinutes(15);
            }
            UpdatedAtUtc = DateTime.UtcNow;
            throw new InvalidOperationException("Invalid PIN");
        }

        FailedPinAttempts = 0;
        PinLockedUntilUtc = null;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Change PIN
    /// </summary>
    public void ChangePin(string newPin)
    {
        if (Status != CardStatus.Active)
            throw new InvalidOperationException("Cannot change PIN on inactive card");

        if (string.IsNullOrEmpty(newPin) || newPin.Length < 4)
            throw new InvalidOperationException("PIN must be at least 4 digits");

        PinHash = HashPin(newPin);
        LastPinChangeUtc = DateTime.UtcNow;
        FailedPinAttempts = 0;
        PinLockedUntilUtc = null;
        UpdatedAtUtc = DateTime.UtcNow;

        RaiseDomainEvent(new CardPinChangedDomainEvent(Id, CustomerId));
    }

    /// <summary>
    /// Validate card for transaction
    /// </summary>
    public void ValidateForTransaction(
        decimal amount,
        TransactionType type,
        bool isInternational = false,
        bool isContactless = false)
    {
        if (Status != CardStatus.Active)
            throw new InvalidOperationException($"Card is {Status}, cannot process transactions");

        if (IsExpired())
        {
            Status = CardStatus.Expired;
            throw new InvalidOperationException("Card has expired");
        }

        if (isContactless && !ContactlessEnabled)
            throw new InvalidOperationException("Contactless transactions are disabled");

        if (isInternational && !InternationalEnabled)
            throw new InvalidOperationException("International transactions are disabled");

        if (!OnlineTransactionsEnabled && type == TransactionType.Transfer)
            throw new InvalidOperationException("Online transactions are disabled");

        // Check daily limits
        ResetDailyLimitsIfNeeded();

        if (type == TransactionType.AtmWithdrawal)
        {
            if (CurrentDayWithdrawalAmount + amount > DailyWithdrawalLimit)
                throw new InvalidOperationException(
                    $"Daily withdrawal limit exceeded. Available: {DailyWithdrawalLimit - CurrentDayWithdrawalAmount}");
        }

        if (isInternational)
        {
            if (CurrentDayTransactionAmount + amount > DailyForeignTransactionLimit)
                throw new InvalidOperationException(
                    $"Daily foreign transaction limit exceeded. Available: {DailyForeignTransactionLimit - CurrentDayTransactionAmount}");
        }
        else
        {
            if (CurrentDayTransactionAmount + amount > DailyTransactionLimit)
                throw new InvalidOperationException(
                    $"Daily transaction limit exceeded. Available: {DailyTransactionLimit - CurrentDayTransactionAmount}");
        }
    }

    /// <summary>
    /// Update daily limits
    /// </summary>
    public void UpdateLimits(decimal dailyLimit, decimal transactionLimit, decimal foreignLimit)
    {
        DailyWithdrawalLimit = dailyLimit;
        DailyTransactionLimit = transactionLimit;
        DailyForeignTransactionLimit = foreignLimit;
        UpdatedAtUtc = DateTime.UtcNow;

        RaiseDomainEvent(new CardLimitsUpdatedDomainEvent(Id, CustomerId, dailyLimit, transactionLimit));
    }

    /// <summary>
    /// Update international transaction settings
    /// </summary>
    public void UpdateInternationalStatus(bool enabled)
    {
        InternationalEnabled = enabled;
        UpdatedAtUtc = DateTime.UtcNow;

        RaiseDomainEvent(new CardSettingsUpdatedDomainEvent(Id, CustomerId, "International", enabled));
    }

    /// <summary>
    /// Check if card is expired
    /// </summary>
    public bool IsExpired() => DateTime.UtcNow >= ExpiryDateUtc;

    /// <summary>
    /// Check if card is expiring soon (30 days)
    /// </summary>
    public bool IsExpiringsoon() => (ExpiryDateUtc - DateTime.UtcNow).TotalDays <= 30;

    /// <summary>
    /// Record a transaction against daily limits
    /// </summary>
    public void RecordTransaction(decimal amount, TransactionType type, bool isInternational)
    {
        ResetDailyLimitsIfNeeded();

        if (type == TransactionType.AtmWithdrawal)
            CurrentDayWithdrawalAmount += amount;
        else if (type == TransactionType.Purchase || type == TransactionType.Transfer)
        {
            if (isInternational)
                CurrentDayTransactionAmount += amount;
            else
                CurrentDayTransactionAmount += amount;
        }

        DailyTransactionCount++;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    // Private helpers

    private void ResetDailyLimitsIfNeeded()
    {
        if (DateTime.UtcNow.Date > LimitResetDateUtc.Date)
        {
            CurrentDayWithdrawalAmount = 0;
            CurrentDayTransactionAmount = 0;
            DailyTransactionCount = 0;
            LimitResetDateUtc = DateTime.UtcNow;
        }
    }

    private static string MaskPan(string cardNumber)
    {
        if (cardNumber.Length < 8)
            return "****";
        
        var first4 = cardNumber.Substring(0, 4);
        var last4 = cardNumber.Substring(cardNumber.Length - 4);
        return $"{first4}-****-****-{last4}";
    }

    private static string HashCvv(string cvv)
    {
        // In production, use BCrypt or Argon2
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(cvv));
        return Convert.ToBase64String(hash);
    }

    private static string HashPin(string pin)
    {
        // In production, use BCrypt or Argon2
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(pin));
        return Convert.ToBase64String(hash);
    }

    private static bool VerifyPin(string pin, string hash)
    {
        // In production, use BCrypt.Net-Next or similar
        var hashOfInput = HashPin(pin);
        return hashOfInput == hash;
    }
}
