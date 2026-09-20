namespace Bank.Statements.Domain.Entities;

using Bank.Statements.Domain.Enums;
using Bank.Statements.Domain.Events;

public class Statement
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid AccountId { get; private set; }
    public DateTime StatementDate { get; private set; }
    public DateTime PeriodStartDate { get; private set; }
    public DateTime PeriodEndDate { get; private set; }
    public decimal OpeningBalance { get; private set; }
    public decimal ClosingBalance { get; private set; }
    public decimal TotalDebits { get; private set; }
    public decimal TotalCredits { get; private set; }
    public StatementStatus Status { get; private set; }
    public StatementPeriod Period { get; private set; }
    public StatementFormat PreferredFormat { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? GeneratedAt { get; private set; }
    public DateTime? SentAt { get; private set; }
    public DateTime? DownloadedAt { get; private set; }
    public DateTime? ArchivedAt { get; private set; }
    public ArchiveReason? ArchiveReason { get; private set; }
    public string? ArchiveNotes { get; private set; }
    public ExportStatus ExportStatus { get; private set; }
    public string? ExportReference { get; private set; }
    public StatementVisibility Visibility { get; private set; }
    public bool IsAccessible { get; private set; }

    public ICollection<StatementLine> Lines { get; private set; } = new List<StatementLine>();
    public ICollection<StatementRecipient> Recipients { get; private set; } = new List<StatementRecipient>();

    private readonly List<object> _domainEvents = new();

    public IReadOnlyList<object> DomainEvents => _domainEvents.AsReadOnly();

    private Statement() { }

    public static Statement Create(
        Guid customerId,
        Guid accountId,
        DateTime periodStartDate,
        DateTime periodEndDate,
        decimal openingBalance,
        StatementPeriod period,
        StatementFormat preferredFormat)
    {
        var statement = new Statement
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            AccountId = accountId,
            StatementDate = DateTime.UtcNow,
            PeriodStartDate = periodStartDate,
            PeriodEndDate = periodEndDate,
            OpeningBalance = openingBalance,
            ClosingBalance = openingBalance,
            Status = StatementStatus.Pending,
            Period = period,
            PreferredFormat = preferredFormat,
            CreatedAt = DateTime.UtcNow,
            ExportStatus = ExportStatus.NotExported,
            Visibility = StatementVisibility.Private,
            IsAccessible = true
        };

        statement.RaiseDomainEvent(new StatementCreatedDomainEvent(statement.Id, customerId, accountId));
        return statement;
    }

    public void Generate(ICollection<StatementLine> lines, decimal totalDebits, decimal totalCredits)
    {
        if (Status != StatementStatus.Pending)
            throw new InvalidOperationException("Statement can only be generated from Pending status");

        Lines = lines;
        TotalDebits = totalDebits;
        TotalCredits = totalCredits;
        ClosingBalance = OpeningBalance - totalDebits + totalCredits;
        Status = StatementStatus.Generated;
        GeneratedAt = DateTime.UtcNow;

        RaiseDomainEvent(new StatementGeneratedDomainEvent(Id, CustomerId, AccountId, ClosingBalance));
    }

    public void Send()
    {
        if (Status != StatementStatus.Generated)
            throw new InvalidOperationException("Statement can only be sent from Generated status");

        Status = StatementStatus.Sent;
        SentAt = DateTime.UtcNow;

        RaiseDomainEvent(new StatementSentDomainEvent(Id, CustomerId, AccountId));
    }

    public void MarkAsDownloaded()
    {
        if (Status == StatementStatus.Cancelled)
            throw new InvalidOperationException("Cancelled statements cannot be downloaded");

        DownloadedAt = DateTime.UtcNow;
        RaiseDomainEvent(new StatementDownloadedDomainEvent(Id, CustomerId, AccountId));
    }

    public void Archive(ArchiveReason reason, string? notes = null)
    {
        if (Status == StatementStatus.Cancelled)
            throw new InvalidOperationException("Cannot archive cancelled statement");

        Status = StatementStatus.Archived;
        ArchivedAt = DateTime.UtcNow;
        ArchiveReason = reason;
        ArchiveNotes = notes;
        IsAccessible = false;

        RaiseDomainEvent(new StatementArchivedDomainEvent(Id, CustomerId, AccountId, reason));
    }

    public void Cancel(string reason)
    {
        if (Status == StatementStatus.Archived)
            throw new InvalidOperationException("Cannot cancel archived statement");

        Status = StatementStatus.Cancelled;
        RaiseDomainEvent(new StatementCancelledDomainEvent(Id, CustomerId, AccountId, reason));
    }

    public void UpdateExportStatus(ExportStatus status, string? reference = null)
    {
        ExportStatus = status;
        ExportReference = reference;

        if (status == ExportStatus.Exported)
        {
            RaiseDomainEvent(new StatementExportedDomainEvent(Id, CustomerId, AccountId, reference!));
        }
    }

    public void SetVisibility(StatementVisibility visibility)
    {
        Visibility = visibility;
        RaiseDomainEvent(new StatementVisibilityChangedDomainEvent(Id, CustomerId, AccountId, visibility));
    }

    public void AddRecipient(StatementRecipient recipient)
    {
        if (Recipients.Any(r => r.Email == recipient.Email))
            throw new InvalidOperationException("Recipient already added to this statement");

        Recipients.Add(recipient);
        RaiseDomainEvent(new StatementRecipientAddedDomainEvent(Id, CustomerId, recipient.Email));
    }

    private void RaiseDomainEvent(object domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
