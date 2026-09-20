namespace Bank.Deposits.Domain.Enums;

/// <summary>
/// Represents the status of a fixed deposit account
/// </summary>
public enum FixedDepositStatus
{
    /// <summary>Active fixed deposit earning interest</summary>
    Active = 1,

    /// <summary>Fixed deposit has reached maturity date</summary>
    Matured = 2,

    /// <summary>Fixed deposit has been closed and funds withdrawn</summary>
    Closed = 3,

    /// <summary>Fixed deposit has been renewed automatically or manually</summary>
    Renewed = 4,
}
