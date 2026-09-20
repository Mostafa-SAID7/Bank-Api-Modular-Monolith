namespace Bank.Statements.Domain.Entities;

using Bank.Statements.Domain.Enums;

public class StatementRecipient
{
    public Guid Id { get; private set; }
    public Guid StatementId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public DeliveryMethod DeliveryMethod { get; private set; }
    public bool HasReceived { get; private set; }
    public DateTime? ReceivedAt { get; private set; }
    public string? DeliveryReference { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private StatementRecipient() { }

    public static StatementRecipient Create(
        Guid statementId,
        string email,
        DeliveryMethod deliveryMethod)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        return new StatementRecipient
        {
            Id = Guid.NewGuid(),
            StatementId = statementId,
            Email = email,
            DeliveryMethod = deliveryMethod,
            HasReceived = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsReceived(string? reference = null)
    {
        HasReceived = true;
        ReceivedAt = DateTime.UtcNow;
        DeliveryReference = reference;
    }
}
