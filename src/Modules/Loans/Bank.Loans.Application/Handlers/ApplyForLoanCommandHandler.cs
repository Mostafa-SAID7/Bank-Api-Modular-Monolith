namespace Bank.Loans.Application.Handlers;

public class ApplyForLoanCommandHandler : IRequestHandler<ApplyForLoanCommand, Loan>
{
    private readonly ILoanProductRepository _loanProductRepo;
    private readonly ILoanRepository _loanRepo;
    private readonly ILoanNumberGenerator _loanNumberGenerator;
    private readonly IEMICalculationService _emiCalculationService;

    public ApplyForLoanCommandHandler(
        ILoanProductRepository loanProductRepo,
        ILoanRepository loanRepo,
        ILoanNumberGenerator loanNumberGenerator,
        IEMICalculationService emiCalculationService)
    {
        _loanProductRepo = loanProductRepo;
        _loanRepo = loanRepo;
        _loanNumberGenerator = loanNumberGenerator;
        _emiCalculationService = emiCalculationService;
    }

    public async Task<Loan> Handle(ApplyForLoanCommand request, CancellationToken cancellationToken)
    {
        var loanProduct = await _loanProductRepo.GetByIdAsync(request.LoanProductId, cancellationToken);
        if (loanProduct == null || !loanProduct.IsActive)
            throw new InvalidOperationException("Loan product not found or inactive");

        if (request.LoanAmount < loanProduct.MinimumAmount || request.LoanAmount > loanProduct.MaximumAmount)
            throw new InvalidOperationException($"Loan amount outside product limits ({loanProduct.MinimumAmount}-{loanProduct.MaximumAmount})");

        if (request.TenureMonths < loanProduct.MinimumTenureMonths || request.TenureMonths > loanProduct.MaximumTenureMonths)
            throw new InvalidOperationException($"Tenure outside product limits ({loanProduct.MinimumTenureMonths}-{loanProduct.MaximumTenureMonths})");

        var loanNumber = await _loanNumberGenerator.GenerateAsync(cancellationToken);
        var emiAmount = _emiCalculationService.CalculateMonthlyEMI(request.LoanAmount, loanProduct.BaseAnnualInterestRate, request.TenureMonths);
        var processingFee = request.LoanAmount * loanProduct.ProcessingFeePercent / 100;

        var loan = Loan.Create(
            loanNumber,
            request.CustomerId,
            request.LoanProductId,
            request.LoanAmount,
            request.TenureMonths,
            loanProduct.BaseAnnualInterestRate,
            loanProduct.InterestType,
            loanProduct.EMIFrequency,
            emiAmount,
            processingFee,
            loanProduct.AllowsPrepayment,
            loanProduct.PrepaymentPenaltyPercent,
            request.CollateralDescription,
            request.CollateralValue);

        await _loanRepo.AddAsync(loan, cancellationToken);
        return loan;
    }
}
