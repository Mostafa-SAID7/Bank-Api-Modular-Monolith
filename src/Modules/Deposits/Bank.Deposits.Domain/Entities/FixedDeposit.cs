using System.Security.Cryptography;

namespace Bank.Deposits.Domain.Entities;

/// <summary>
/// Represents a fixed deposit account with term locking and maturity handling
/// </summary>
public class FixedDeposit
{
    public Guid Id { get; private set; }
    public string DepositNumber { get; private set; } = string.Empty;
    public Guid CustomerId { get; private set; }
    public Guid DepositProductId { get; private set; }
    public Guid LinkedAccountId { get; private set; }
    
    // Deposit details
    public decimal PrincipalAmount { get; private set; }
    public decimal InterestRate { get; private set; }
    public int TermDays { get; private set; }
    public DateTime StartDateUtc { get; private set; }
    public DateTime MaturityDateUtc { get; private set; }
    public FixedDepositStatus Status { get; private set; }
    
    // Interest calculation
    public InterestCalculationMethod InterestCalculationMethod { get; private set; }
    public InterestFrequency CompoundingFrequency { get; private set; }
    public decimal AccruedInterest { get; private set; }
    public DateTime LastInterestCalculationDateUtc { get; private set; }
    
    // Maturity settings
    public MaturityAction MaturityAction { get; private set; }
    public bool AutoRenewalEnabled { get; private set; }
    public int? RenewalTermDays { get; private set; }
    public DateTime? RenewalNoticeDateUtc { get; private set; }
    public bool CustomerConsentReceived { get; private set; }
    
    // Penalty settings
    public WithdrawalPenaltyType PenaltyType { get; private set; }
    public decimal? PenaltyAmount { get; private set; }
    public decimal? PenaltyPercentage { get; private set; }
    
    // Closure details
    public DateTime? ClosureDateUtc { get; private set; }
    public Guid? ClosedByUserId { get; private set; }
    public string? ClosureReason { get; private set; }
    public decimal? PenaltyApplied { get; private set; }
    public decimal? NetAmountPaid { get; private set; }
    
    // Renewal tracking
    public Guid? RenewedFromDepositId { get; private set; }
    public Guid? RenewedToDepositId { get; private set; }
    public int RenewalCount { get; private set; }
    
    // Audit timestamps
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    /// <summary>
    /// Create a new fixed deposit
    /// </summary>
    public static FixedDeposit Create(
        Guid customerId,
        Guid linkedAccountId,
        Guid depositProductId,
        decimal principalAmount,
        int termDays,
        decimal interestRate,
        InterestCalculationMethod calculationMethod,
        InterestFrequency compoundingFrequency,
        WithdrawalPenaltyType penaltyType,
        decimal? penaltyAmount = null,
        decimal? penaltyPercentage = null)
    {
        var fixedDeposit = new FixedDeposit
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            LinkedAccountId = linkedAccountId,
            DepositProductId = depositProductId,
            PrincipalAmount = principalAmount,
            TermDays = termDays,
            InterestRate = interestRate,
            InterestCalculationMethod = calculationMethod,
            CompoundingFrequency = compoundingFrequency,
            StartDateUtc = DateTime.UtcNow,
            MaturityDateUtc = DateTime.UtcNow.AddDays(termDays),
            Status = FixedDepositStatus.Active,
            AccruedInterest = 0,
            LastInterestCalculationDateUtc = DateTime.UtcNow,
            MaturityAction = MaturityAction.Payout,
            AutoRenewalEnabled = false,
            CustomerConsentReceived = false,
            PenaltyType = penaltyType,
            PenaltyAmount = penaltyAmount,
            PenaltyPercentage = penaltyPercentage,
            RenewalCount = 0,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        };

        fixedDeposit.GenerateDepositNumber();
        return fixedDeposit;
    }

    /// <summary>
    /// Calculates the maturity amount including principal and interest
    /// </summary>
    public decimal CalculateMaturityAmount()
    {
        return PrincipalAmount + CalculateInterestAtMaturity();
    }
    
    /// <summary>
    /// Calculates the total interest at maturity
    /// </summary>
    public decimal CalculateInterestAtMaturity()
    {
        var principal = PrincipalAmount;
        var rate = InterestRate / 100;
        var termYears = TermDays / 365.0m;
        
        return InterestCalculationMethod switch
        {
            InterestCalculationMethod.Simple => principal * rate * termYears,
            InterestCalculationMethod.CompoundDaily => CalculateCompoundInterest(365),
            InterestCalculationMethod.CompoundMonthly => CalculateCompoundInterest(12),
            _ => principal * rate * termYears
        };
    }
    
    /// <summary>
    /// Calculates compound interest based on compounding frequency
    /// </summary>
    private decimal CalculateCompoundInterest(int compoundingPerYear)
    {
        var principal = PrincipalAmount;
        var rate = InterestRate / 100;
        var termYears = TermDays / 365.0m;
        
        var amount = principal * (decimal)Math.Pow((double)(1 + rate / compoundingPerYear), (double)(compoundingPerYear * termYears));
        return amount - principal;
    }
    
    /// <summary>
    /// Calculates early withdrawal penalty
    /// </summary>
    public decimal CalculateEarlyWithdrawalPenalty(decimal withdrawalAmount)
    {
        if (Status != FixedDepositStatus.Active)
            return 0;
            
        return PenaltyType switch
        {
            WithdrawalPenaltyType.None => 0,
            WithdrawalPenaltyType.FixedAmount => PenaltyAmount ?? 0,
            WithdrawalPenaltyType.Percentage => withdrawalAmount * (PenaltyPercentage ?? 0) / 100,
            WithdrawalPenaltyType.InterestForfeiture => AccruedInterest,
            WithdrawalPenaltyType.Combined => (PenaltyAmount ?? 0) + (withdrawalAmount * (PenaltyPercentage ?? 0) / 100),
            _ => 0
        };
    }
    
    /// <summary>
    /// Checks if the deposit has matured
    /// </summary>
    public bool HasMatured()
    {
        return DateTime.UtcNow >= MaturityDateUtc && Status == FixedDepositStatus.Active;
    }
    
    /// <summary>
    /// Checks if renewal notice should be sent
    /// </summary>
    public bool ShouldSendRenewalNotice()
    {
        if (!AutoRenewalEnabled || RenewalNoticeDateUtc.HasValue)
            return false;
            
        // Default 30 days before maturity
        var noticeDays = 30;
        var noticeDate = MaturityDateUtc.AddDays(-noticeDays);
        
        return DateTime.UtcNow >= noticeDate;
    }

    /// <summary>
    /// Mark the deposit as matured
    /// </summary>
    public void MarkAsMatured()
    {
        if (Status != FixedDepositStatus.Active)
            throw new InvalidOperationException("Only active deposits can be marked as matured");
        
        Status = FixedDepositStatus.Matured;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Close the fixed deposit
    /// </summary>
    public void Close(string? reason = null)
    {
        if (Status == FixedDepositStatus.Closed)
            throw new InvalidOperationException("Deposit is already closed");
        
        Status = FixedDepositStatus.Closed;
        ClosureDateUtc = DateTime.UtcNow;
        ClosureReason = reason;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Enable auto-renewal
    /// </summary>
    public void EnableAutoRenewal(int renewalTermDays, bool requireConsent = true)
    {
        if (Status != FixedDepositStatus.Active && Status != FixedDepositStatus.Matured)
            throw new InvalidOperationException("Cannot enable renewal for this deposit");
        
        AutoRenewalEnabled = true;
        RenewalTermDays = renewalTermDays;
        CustomerConsentReceived = !requireConsent;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Renew the fixed deposit
    /// </summary>
    public FixedDeposit Renew(int newTermDays)
    {
        if (Status != FixedDepositStatus.Matured)
            throw new InvalidOperationException("Only matured deposits can be renewed");
        
        var renewedDeposit = Create(
            CustomerId,
            LinkedAccountId,
            DepositProductId,
            PrincipalAmount + AccruedInterest, // Roll over principal + accrued interest
            newTermDays,
            InterestRate,
            InterestCalculationMethod,
            CompoundingFrequency,
            PenaltyType,
            PenaltyAmount,
            PenaltyPercentage);

        renewedDeposit.RenewedFromDepositId = Id;
        RenewedToDepositId = renewedDeposit.Id;
        RenewalCount++;
        Status = FixedDepositStatus.Renewed;
        UpdatedAtUtc = DateTime.UtcNow;

        return renewedDeposit;
    }

    /// <summary>
    /// Record accrued interest
    /// </summary>
    public void RecordAccruedInterest(decimal interestAmount)
    {
        if (Status != FixedDepositStatus.Active)
            throw new InvalidOperationException("Cannot accrue interest on inactive deposits");
        
        if (interestAmount < 0)
            throw new InvalidOperationException("Interest amount cannot be negative");
        
        AccruedInterest += interestAmount;
        LastInterestCalculationDateUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Generates a unique deposit number
    /// </summary>
    private void GenerateDepositNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd");
        using var rng = RandomNumberGenerator.Create();
        var randomBytes = new byte[4];
        rng.GetBytes(randomBytes);
        var random = Math.Abs(BitConverter.ToInt32(randomBytes, 0)) % 9000 + 1000;
        DepositNumber = $"FD{timestamp}{random}";
    }
}
