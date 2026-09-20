namespace Bank.Identity.Domain.Enums;

/// <summary>
/// Session status
/// </summary>
public enum SessionStatus
{
    Active = 1,
    Expired = 2,
    Terminated = 3,
    Locked = 4
}
