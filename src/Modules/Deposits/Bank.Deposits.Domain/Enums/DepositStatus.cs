namespace Bank.Deposits.Domain.Enums;

/// <summary>
/// Deposit account status enumeration
/// </summary>
public enum DepositStatus
{
    /// <summary>Active deposit account</summary>
    Active = 0,

    /// <summary>Frozen/blocked deposit account</summary>
    Frozen = 1,

    /// <summary>Closed deposit account</summary>
    Closed = 2,

    /// <summary>Matured fixed deposit awaiting withdrawal</summary>
    Matured = 3
}
