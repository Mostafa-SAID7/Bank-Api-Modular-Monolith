namespace Bank.Statements.Domain.Entities;

public class StatementFormatEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string FileExtension { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private StatementFormatEntity() { }

    public static StatementFormatEntity Create(
        string name,
        string description,
        string fileExtension,
        string contentType,
        int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        return new StatementFormatEntity
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            FileExtension = fileExtension,
            ContentType = contentType,
            IsActive = true,
            DisplayOrder = displayOrder,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
