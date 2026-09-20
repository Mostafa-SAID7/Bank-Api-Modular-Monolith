namespace Bank.Loans.Domain.Entities;

/// <summary>
/// Loan aggregate root
/// Manages entire loan lifecycle from application to closure
/// Enforces all business rules and invariants
/// </summary>
public class Loan
{
    // Aggregate root properties
    public Guid Id { get; private set; }
    public string LoanNumber { get; private set; } = string.Empty;
    public Guid CustomerId { get; private set; }
    public Guid LoanProductId { get; private set; }
    
    // Loan amount and tenure
    public decimal LoanAmount { get; private set; }
    public decimal ProcessingFee { get; private set; }
    public decimal NetDisbursedAmount { get; private set; } // LoanAmount - ProcessingFee
    public int TenureMonths { get; private set; }
    
    // Interest and payment details
    public decimal AnnualInterestRate { get; private set; }
    public int InterestType { get; private set; } // InterestType enum
    public int EMIFrequency { get; private set; } // EMIFrequency enum
    public decimal EMIAmount { get; private set; }
    
    // Loan status and timeline
    public int Status { get; private set; } // LoanStatus enum
    public DateTime ApplicationDateUtc { get; private set; }
    public DateTime? ApprovalDateUtc { get; private set; }
    public DateTime? DisbursementDateUtc { get; private set; }
    public DateTime? ClosureDateUtc { get; private set; }
    public DateTime? RejectionDateUtc { get; private set; }
    
    // Approval details
    public string? ApprovalNotes { get; private set; }
    public Guid? ApprovedByUserId { get; private set; }
    
    // Repayment tracking
    public decimal TotalPrincipalRepaid { get; private set; }
    public decimal TotalInterestRepaid { get; private set; }
    public decimal TotalPenaltyCharges { get; private set; }
    
    // Outstanding amounts
    public decimal OutstandingPrincipal { get; private set; }
    public decimal OutstandingInterest { get; private set; }
    
    // Collateral and security
    public string? CollateralDescription { get; private set; }
    public decimal? CollateralValue { get; private set; }
    
    // Prepayment
    public bool AllowsPrepayment { get; private set; }
    public decimal? PrepaymentPenaltyPercent { get; private set; }
    
    // Last update
    public DateTime UpdatedAtUtc { get; private set; }

    // Domain events
    private readonly List<object> _domainEvents = new();
    public IReadOnlyList<object> DomainEvents => _domainEvents.AsReadOnly();

    #region Factory Method

    /// <summary>
    /// Factory method to create a new loan application
    /// </summary>
    public static Loan Create(
        string loanNumber,
        Guid customerId,
        Guid loanProductId,
        decimal loanAmount,
        int tenureMonths,
        decimal annualInterestRate,
        int interestType,
        int emiFrequency,
        decimal emiAmount,
        decimal processingFee,
        bool allowsPrepayment,
        decimal? prepaymentPenaltyPercent,
        string? collateralDescription = null,
        decimal? collateralValue = null)
    {
        // Validate inputs
        if (loanAmount <= 0)
            throw new InvalidOperationException("Loan amount must be greater than zero");
        
        if (tenureMonths <= 0)
            throw new InvalidOperationException("Tenure must be greater than zero");
        
        if (annualInterestRate < 0)
            throw new InvalidOperationException("Interest rate cannot be negative");
        
        if (emiAmount <= 0)
            throw new InvalidOperationException("EMI amount must be greater than zero");

        var loan = new Loan
        {
            Id = Guid.NewGuid(),
            LoanNumber = loanNumber,
            CustomerId = customerId,
            LoanProductId = loanProductId,
            LoanAmount = loanAmount,
            ProcessingFee = processingFee,
            NetDisbursedAmount = loanAmount - processingFee,
            TenureMonths = tenureMonths,
            AnnualInterestRate = annualInterestRate,
            InterestType = interestType,
            EMIFrequency = emiFrequency,
            EMIAmount = emiAmount,
            Status = (int)Enums.LoanStatus.Applied,
            ApplicationDateUtc = DateTime.UtcNow,
            AllowsPrepayment = allowsPrepayment,
            PrepaymentPenaltyPercent = prepaymentPenaltyPercent,
            CollateralDescription = collateralDescription,
            CollateralValue = collateralValue,
            OutstandingPrincipal = loanAmount,
            OutstandingInterest = 0,
            UpdatedAtUtc = DateTime.UtcNow
        };

        // Raise domain event
        loan._domainEvents.Add(new Events.LoanApplicationSubmittedDomainEvent(
            loan.Id, loan.CustomerId, loan.LoanNumber, loan.LoanAmount, loan.TenureMonths));

        return loan;
    }

    #endregion

    #region Domain Methods

    /// <summary>
    /// Approve the loan application
    /// </summary>
    public void Approve(Guid approvedByUserId, string? notes = null)
    {
        if (Status != (int)Enums.LoanStatus.Applied)
            throw new InvalidOperationException($"Cannot approve loan in status {(Enums.LoanStatus)Status}");

        Status = (int)Enums.LoanStatus.Approved;
        ApprovalDateUtc = DateTime.UtcNow;
        ApprovedByUserId = approvedByUserId;
        ApprovalNotes = notes;
        UpdatedAtUtc = DateTime.UtcNow;

        _domainEvents.Add(new Events.LoanApprovedDomainEvent(
            Id, CustomerId, LoanNumber, LoanAmount, approvedByUserId));
    }

    /// <summary>
    /// Reject the loan application
    /// </summary>
    public void Reject(string reason)
    {
        if (Status != (int)Enums.LoanStatus.Applied)
            throw new InvalidOperationException($"Cannot reject loan in status {(Enums.LoanStatus)Status}");

        Status = (int)Enums.LoanStatus.Rejected;
        RejectionDateUtc = DateTime.UtcNow;
        ApprovalNotes = reason;
        UpdatedAtUtc = DateTime.UtcNow;

        _domainEvents.Add(new Events.LoanRejectedDomainEvent(
            Id, CustomerId, LoanNumber, reason));
    }

    /// <summary>
    /// Disburse the loan amount to customer
    /// </summary>
    public void Disburse(string? referenceNumber = null)
    {
        if (Status != (int)Enums.LoanStatus.Approved)
            throw new InvalidOperationException($"Cannot disburse loan in status {(Enums.LoanStatus)Status}");

        Status = (int)Enums.LoanStatus.Disbursed;
        DisbursementDateUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;

        _domainEvents.Add(new Events.LoanDisbursedDomainEvent(
            Id, CustomerId, LoanNumber, NetDisbursedAmount, referenceNumber ?? string.Empty));
    }

    /// <summary>
    /// Activate the loan for payment collection (after disbursement)
    /// </summary>
    public void Activate()
    {
        if (Status != (int)Enums.LoanStatus.Disbursed)
            throw new InvalidOperationException($"Cannot activate loan in status {(Enums.LoanStatus)Status}");

        Status = (int)Enums.LoanStatus.Active;
        UpdatedAtUtc = DateTime.UtcNow;

        _domainEvents.Add(new Events.LoanActivatedDomainEvent(Id, CustomerId, LoanNumber));
    }

    /// <summary>
    /// Record EMI payment
    /// </summary>
    public void RecordPayment(decimal principalPaid, decimal interestPaid, decimal? penaltyPaid = null)
    {
        if (Status != (int)Enums.LoanStatus.Active && Status != (int)Enums.LoanStatus.Disbursed)
            throw new InvalidOperationException($"Cannot record payment on loan in status {(Enums.LoanStatus)Status}");

        if (principalPaid < 0 || interestPaid < 0)
            throw new InvalidOperationException("Payment amounts cannot be negative");

        TotalPrincipalRepaid += principalPaid;
        TotalInterestRepaid += interestPaid;

        if (penaltyPaid.HasValue && penaltyPaid.Value > 0)
        {
            TotalPenaltyCharges += penaltyPaid.Value;
        }

        OutstandingPrincipal = LoanAmount - TotalPrincipalRepaid;
        OutstandingInterest = (LoanAmount * AnnualInterestRate / 100 * TenureMonths / 12) - TotalInterestRepaid;

        UpdatedAtUtc = DateTime.UtcNow;

        _domainEvents.Add(new Events.EMIPaymentRecordedDomainEvent(
            Id, CustomerId, LoanNumber, principalPaid, interestPaid, OutstandingPrincipal));
    }

    /// <summary>
    /// Close the loan after full repayment
    /// </summary>
    public void Close()
    {
        if (Status == (int)Enums.LoanStatus.Closed)
            throw new InvalidOperationException("Loan is already closed");

        if (OutstandingPrincipal > 0)
            throw new InvalidOperationException("Cannot close loan with outstanding principal");

        Status = (int)Enums.LoanStatus.Closed;
        ClosureDateUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;

        _domainEvents.Add(new Events.LoanClosedDomainEvent(
            Id, CustomerId, LoanNumber, TotalPrincipalRepaid, TotalInterestRepaid, TotalPenaltyCharges));
    }

    /// <summary>
    /// Mark loan as defaulted due to missed payments
    /// </summary>
    public void MarkAsDefaulted(string reason)
    {
        if (Status == (int)Enums.LoanStatus.Closed)
            throw new InvalidOperationException("Cannot default a closed loan");

        Status = (int)Enums.LoanStatus.Defaulted;
        UpdatedAtUtc = DateTime.UtcNow;

        _domainEvents.Add(new Events.LoanDefaultedDomainEvent(
            Id, CustomerId, LoanNumber, OutstandingPrincipal, reason));
    }

    /// <summary>
    /// Cancel loan application before disbursement
    /// </summary>
    public void Cancel(string reason)
    {
        if (Status != (int)Enums.LoanStatus.Approved && Status != (int)Enums.LoanStatus.Applied)
            throw new InvalidOperationException($"Cannot cancel loan in status {(Enums.LoanStatus)Status}");

        Status = (int)Enums.LoanStatus.Cancelled;
        UpdatedAtUtc = DateTime.UtcNow;

        _domainEvents.Add(new Events.LoanCancelledDomainEvent(
            Id, CustomerId, LoanNumber, reason));
    }

    /// <summary>
    /// Prepay the loan (partial or full)
    /// </summary>
    public decimal Prepay(decimal amount)
    {
        if (!AllowsPrepayment)
            throw new InvalidOperationException("This loan does not allow prepayment");

        if (Status != (int)Enums.LoanStatus.Active)
            throw new InvalidOperationException($"Cannot prepay loan in status {(Enums.LoanStatus)Status}");

        if (amount <= 0 || amount > OutstandingPrincipal)
            throw new InvalidOperationException("Prepayment amount invalid");

        var penaltyCharge = 0m;
        if (PrepaymentPenaltyPercent.HasValue && PrepaymentPenaltyPercent > 0)
        {
            penaltyCharge = amount * PrepaymentPenaltyPercent.Value / 100;
        }

        OutstandingPrincipal -= amount;
        TotalPrincipalRepaid += amount;
        UpdatedAtUtc = DateTime.UtcNow;

        _domainEvents.Add(new Events.LoanPrepaidDomainEvent(
            Id, CustomerId, LoanNumber, amount, penaltyCharge, OutstandingPrincipal));

        return penaltyCharge;
    }

    #endregion
}
