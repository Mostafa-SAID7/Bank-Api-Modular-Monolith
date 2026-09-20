using Bank.Deposits.Domain.Enums;

namespace Bank.Deposits.Domain.Entities;

/// <summary>
/// Deposit aggregate root - Represents a customer's deposit account
/// Manages deposit lifecycle: opening, deposits, withdrawals, interest accrual, maturity, closure
/// </summary>
public class Deposit
{
    /// <summary>Aggregate root Id</summary>
    public Guid Id { get; private set; }
    public string AccountNumber { get; private set; } = null!;

    /// <summary>Customer ID who owns this deposit</summary>
    public Guid CustomerId { get; private set; }

    /// <summary>Deposit type reference</summary>
    public Guid DepositTypeId { get; private set; }

    /// <summary>Navigation property to deposit type</summary>
    public DepositType DepositType { get; private set; } = null!;

    /// <summary>Current balance in deposit account</summary>
    public decimal CurrentBalance { get; private set; }

    /// <summary>Total interest accrued (not yet paid)</summary>
    public decimal AccruedInterest { get; private set; }

    /// <summary>Total interest paid so far</summary>
    public decimal PaidInterest { get; private set; }

    /// <summary>Deposit status (Active, Frozen, Closed, Matured)</summary>
    public DepositStatus Status { get; private set; }

    /// <summary>Opening date of the deposit account</summary>
    public DateTime OpenedAtUtc { get; private set; }

    /// <summary>For fixed deposits, the maturity date</summary>
    public DateTime? MaturityDateUtc { get; private set; }

    /// <summary>Closure date if account is closed</summary>
    public DateTime? ClosedAtUtc { get; private set; }

    /// <summary>Last interest accrual date</summary>
    public DateTime? LastInterestAccrualUtc { get; private set; }

    /// <summary>Next scheduled interest payment date</summary>
    public DateTime? NextInterestPaymentUtc { get; private set; }

    /// <summary>Is this a fixed term deposit</summary>
    public bool IsFixedDeposit { get; private set; }

    /// <summary>Term in months for fixed deposits</summary>
    public int TermMonths { get; private set; }

    /// <summary>Interest frequency for this deposit</summary>
    public InterestFrequency InterestFrequency { get; private set; }

    /// <summary>Reason for account freeze (if applicable)</summary>
    public string? FreezeReason { get; private set; }

    /// <summary>Reason for account closure (if applicable)</summary>
    public string? ClosureReason { get; private set; }

    /// <summary>Latest update timestamp</summary>
    public DateTime UpdatedAtUtc { get; private set; }

    // Domain events
    private readonly List<object> _domainEvents = new();
    public IReadOnlyList<object> DomainEvents => _domainEvents.AsReadOnly();

    private void AddDomainEvent(object domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Factory method to create a new deposit account
    /// </summary>
    public static Deposit Create(
        string accountNumber,
        Guid customerId,
        Guid depositTypeId,
        DepositType depositType,
        decimal initialDeposit,
        bool isFixedDeposit,
        int termMonths,
        InterestFrequency interestFrequency)
    {
        var deposit = new Deposit
        {
            Id = Guid.NewGuid(),
            AccountNumber = accountNumber,
            CustomerId = customerId,
            DepositTypeId = depositTypeId,
            DepositType = depositType,
            CurrentBalance = initialDeposit,
            AccruedInterest = 0,
            PaidInterest = 0,
            Status = DepositStatus.Active,
            OpenedAtUtc = DateTime.UtcNow,
            MaturityDateUtc = isFixedDeposit ? DateTime.UtcNow.AddMonths(termMonths) : null,
            ClosedAtUtc = null,
            LastInterestAccrualUtc = null,
            NextInterestPaymentUtc = DateTime.UtcNow.AddMonths(1),
            IsFixedDeposit = isFixedDeposit,
            TermMonths = termMonths,
            InterestFrequency = interestFrequency,
            FreezeReason = null,
            ClosureReason = null,
            UpdatedAtUtc = DateTime.UtcNow
        };

        // Raise domain event
        deposit.AddDomainEvent(new DepositOpenedDomainEvent(
            deposit.Id,
            customerId,
            accountNumber,
            initialDeposit,
            DateTime.UtcNow));

        return deposit;
    }

    /// <summary>
    /// Add funds into the account
    /// </summary>
    public void AddFunds(decimal amount, string description)
    {
        if (Status != DepositStatus.Active)
            throw new InvalidOperationException($"Cannot deposit into an account with status {Status}");

        if (amount <= 0)
            throw new InvalidOperationException("Deposit amount must be greater than zero");

        if (DepositType.MaximumBalance > 0 && CurrentBalance + amount > DepositType.MaximumBalance)
            throw new InvalidOperationException($"Deposit would exceed maximum balance of {DepositType.MaximumBalance}");

        CurrentBalance += amount;
        UpdatedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new DepositFundsAddedDomainEvent(
            Id,
            CustomerId,
            AccountNumber,
            amount,
            CurrentBalance,
            DateTime.UtcNow));
    }

    /// <summary>
    /// Withdraw funds from the account
    /// </summary>
    public void Withdraw(decimal amount, string reason)
    {
        if (Status == DepositStatus.Closed)
            throw new InvalidOperationException("Cannot withdraw from a closed account");

        if (Status == DepositStatus.Frozen)
            throw new InvalidOperationException("Cannot withdraw from a frozen account");

        if (amount <= 0)
            throw new InvalidOperationException("Withdrawal amount must be greater than zero");

        if (amount > CurrentBalance)
            throw new InvalidOperationException($"Insufficient balance. Available: {CurrentBalance}, Requested: {amount}");

        if (IsFixedDeposit && Status != DepositStatus.Matured)
            throw new InvalidOperationException("Cannot withdraw from fixed deposit before maturity");

        CurrentBalance -= amount;
        UpdatedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new WithdrawalMadeDomainEvent(
            Id,
            CustomerId,
            AccountNumber,
            amount,
            CurrentBalance,
            reason,
            DateTime.UtcNow));
    }

    /// <summary>
    /// Freeze the deposit account (block withdrawals)
    /// </summary>
    public void Freeze(string reason)
    {
        if (Status == DepositStatus.Closed)
            throw new InvalidOperationException("Cannot freeze a closed account");

        Status = DepositStatus.Frozen;
        FreezeReason = reason;
        UpdatedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new DepositFrozenDomainEvent(
            Id,
            CustomerId,
            AccountNumber,
            reason,
            DateTime.UtcNow));
    }

    /// <summary>
    /// Unfreeze the deposit account
    /// </summary>
    public void Unfreeze()
    {
        if (Status != DepositStatus.Frozen)
            throw new InvalidOperationException("Account is not frozen");

        Status = DepositStatus.Active;
        FreezeReason = null;
        UpdatedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new DepositUnfrozenDomainEvent(
            Id,
            CustomerId,
            AccountNumber,
            DateTime.UtcNow));
    }

    /// <summary>
    /// Accrue interest to the account
    /// </summary>
    public void AccrueInterest(decimal interestAmount)
    {
        if (interestAmount < 0)
            throw new InvalidOperationException("Interest amount cannot be negative");

        if (Status == DepositStatus.Closed)
            throw new InvalidOperationException("Cannot accrue interest on closed account");

        AccruedInterest += interestAmount;
        LastInterestAccrualUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new InterestAccruedDomainEvent(
            Id,
            CustomerId,
            AccountNumber,
            interestAmount,
            AccruedInterest,
            DateTime.UtcNow));
    }

    /// <summary>
    /// Pay out accrued interest to the account
    /// </summary>
    public void PayInterest()
    {
        if (AccruedInterest <= 0)
            throw new InvalidOperationException("No accrued interest to pay");

        var interestPaid = AccruedInterest;
        CurrentBalance += AccruedInterest;
        PaidInterest += AccruedInterest;
        AccruedInterest = 0;
        UpdatedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new InterestPaidDomainEvent(
            Id,
            CustomerId,
            AccountNumber,
            interestPaid,
            CurrentBalance,
            DateTime.UtcNow));
    }

    /// <summary>
    /// Mark fixed deposit as matured
    /// </summary>
    public void MarkAsMatured()
    {
        if (!IsFixedDeposit)
            throw new InvalidOperationException("Only fixed deposits can be marked as matured");

        if (Status == DepositStatus.Closed)
            throw new InvalidOperationException("Cannot mature a closed account");

        Status = DepositStatus.Matured;
        UpdatedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new DepositMaturedDomainEvent(
            Id,
            CustomerId,
            AccountNumber,
            CurrentBalance,
            DateTime.UtcNow));
    }

    /// <summary>
    /// Close the deposit account
    /// </summary>
    public void Close(string reason)
    {
        if (Status == DepositStatus.Closed)
            throw new InvalidOperationException("Account is already closed");

        Status = DepositStatus.Closed;
        ClosedAtUtc = DateTime.UtcNow;
        ClosureReason = reason;
        UpdatedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new DepositClosedDomainEvent(
            Id,
            CustomerId,
            AccountNumber,
            CurrentBalance,
            PaidInterest + AccruedInterest,
            reason,
            DateTime.UtcNow));
    }
}
