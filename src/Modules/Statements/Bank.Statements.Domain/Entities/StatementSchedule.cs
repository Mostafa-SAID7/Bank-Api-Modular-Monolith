namespace Bank.Statements.Domain.Entities;

using Bank.Statements.Domain.Enums;

public class StatementSchedule
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid AccountId { get; private set; }
    public StatementPeriod Period { get; private set; }
    public StatementFormat PreferredFormat { get; private set; }
    public DeliveryMethod DeliveryMethod { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? NextGenerationDate { get; private set; }
    public DateTime? LastGeneratedDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private StatementSchedule() { }

    public static StatementSchedule Create(
        Guid customerId,
        Guid accountId,
        StatementPeriod period,
        StatementFormat format,
        DeliveryMethod deliveryMethod)
    {
        return new StatementSchedule
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            AccountId = accountId,
            Period = period,
            PreferredFormat = format,
            DeliveryMethod = deliveryMethod,
            IsActive = true,
            NextGenerationDate = CalculateNextGenerationDate(period),
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateSchedule(StatementPeriod period, StatementFormat format, DeliveryMethod deliveryMethod)
    {
        Period = period;
        PreferredFormat = format;
        DeliveryMethod = deliveryMethod;
        UpdatedAt = DateTime.UtcNow;
        NextGenerationDate = CalculateNextGenerationDate(period);
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        NextGenerationDate = CalculateNextGenerationDate(Period);
    }

    public void MarkAsGenerated()
    {
        LastGeneratedDate = DateTime.UtcNow;
        NextGenerationDate = CalculateNextGenerationDate(Period);
        UpdatedAt = DateTime.UtcNow;
    }

    private static DateTime CalculateNextGenerationDate(StatementPeriod period)
    {
        var today = DateTime.UtcNow.Date;
        return period switch
        {
            StatementPeriod.Daily => today.AddDays(1),
            StatementPeriod.Weekly => today.AddDays(7),
            StatementPeriod.BiWeekly => today.AddDays(14),
            StatementPeriod.Monthly => today.AddMonths(1),
            StatementPeriod.Quarterly => today.AddMonths(3),
            StatementPeriod.HalfYearly => today.AddMonths(6),
            StatementPeriod.Yearly => today.AddYears(1),
            _ => today.AddMonths(1)
        };
    }
}
