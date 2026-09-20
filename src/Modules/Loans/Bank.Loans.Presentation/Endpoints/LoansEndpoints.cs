namespace Bank.Loans.Presentation.Endpoints;

/// <summary>
/// REST API endpoints for Loans module
/// Handles loan application, approval, disbursement, and payment operations
/// </summary>
public static class LoansEndpoints
{
    public const string Base = "api/v1/loans";

    public static void MapLoansEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(Base)
            .WithTags("Loans");

        group.MapPost("/apply", ApplyForLoan)
            .WithName("ApplyForLoan")
            .WithOpenApi()
            .WithDescription("Submit a new loan application");

        group.MapPost("/{loanId}/approve", ApproveLoan)
            .WithName("ApproveLoan")
            .WithOpenApi()
            .WithDescription("Approve a pending loan application");

        group.MapPost("/{loanId}/reject", RejectLoan)
            .WithName("RejectLoan")
            .WithOpenApi()
            .WithDescription("Reject a loan application");

        group.MapPost("/{loanId}/disburse", DisburseLoans)
            .WithName("DisburseLoans")
            .WithOpenApi()
            .WithDescription("Disburse an approved loan");

        group.MapPost("/{loanId}/payment", RecordPayment)
            .WithName("RecordPayment")
            .WithOpenApi()
            .WithDescription("Record an EMI payment");

        group.MapPost("/{loanId}/prepay", PrepayLoan)
            .WithName("PrepayLoan")
            .WithOpenApi()
            .WithDescription("Make a prepayment on a loan");

        group.MapPost("/{loanId}/close", CloseLoan)
            .WithName("CloseLoan")
            .WithOpenApi()
            .WithDescription("Close a loan");

        group.MapGet("/{loanId}", GetLoanById)
            .WithName("GetLoanById")
            .WithOpenApi()
            .WithDescription("Get loan details by ID");

        group.MapGet("/customer/{customerId}", GetCustomerLoans)
            .WithName("GetCustomerLoans")
            .WithOpenApi()
            .WithDescription("Get all loans for a customer");

        group.MapPost("/calculate-emi", CalculateEMI)
            .WithName("CalculateEMI")
            .WithOpenApi()
            .WithDescription("Calculate EMI for given loan parameters");

        group.MapGet("/{loanId}/schedule", GetLoanSchedule)
            .WithName("GetLoanSchedule")
            .WithOpenApi()
            .WithDescription("Get amortization schedule for a loan");
    }

    private static async Task<IResult> ApplyForLoan(
        CreateLoanRequest request,
        IMediator mediator,
        IValidator<CreateLoanRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        var command = new ApplyForLoanCommand(
            request.CustomerId,
            request.LoanProductId,
            request.LoanAmount,
            request.TenureMonths,
            request.CollateralDescription,
            request.CollateralValue);

        try
        {
            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(new
            {
                loanId = result,
                message = "Loan application submitted successfully"
            });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> ApproveLoan(
        Guid loanId,
        ApproveLoanRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new ApproveLoanCommand(loanId, request.ApprovedByUserId, request.ApprovalNotes);
        try
        {
            await mediator.Send(command, cancellationToken);
            return Results.Ok(new { message = "Loan approved successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> RejectLoan(
        Guid loanId,
        RejectLoanRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new RejectLoanCommand(loanId, request.RejectionReason);
        try
        {
            await mediator.Send(command, cancellationToken);
            return Results.Ok(new { message = "Loan rejected successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> DisburseLoans(
        Guid loanId,
        DisburseLoanRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new DisburseLoansCommand(loanId, request.DisbursementReferenceNumber);
        try
        {
            await mediator.Send(command, cancellationToken);
            return Results.Ok(new { message = "Loan disbursed successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> RecordPayment(
        Guid loanId,
        RecordPaymentRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new RecordEMIPaymentCommand(request.LoanId, request.Amount, request.ScheduleId);
        try
        {
            await mediator.Send(command, cancellationToken);
            return Results.Ok(new { message = "Payment recorded successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> PrepayLoan(
        Guid loanId,
        PrepayLoanRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new PrepayLoanCommand(loanId, request.PrepaymentAmount);
        try
        {
            await mediator.Send(command, cancellationToken);
            return Results.Ok(new { message = "Prepayment processed successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> CloseLoan(
        Guid loanId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CloseLoanCommand(loanId);
        try
        {
            await mediator.Send(command, cancellationToken);
            return Results.Ok(new { message = "Loan closed successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> GetLoanById(
        Guid loanId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetLoanByIdQuery(loanId);
        try
        {
            var result = await mediator.Send(query, cancellationToken);
            return Results.Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { error = ex.Message });
        }
    }

    private static async Task<IResult> GetCustomerLoans(
        Guid customerId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        IMediator mediator = null!,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCustomerLoansQuery(customerId, page, pageSize);
        try
        {
            var result = await mediator.Send(query, cancellationToken);
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> CalculateEMI(
        CalculateEMIQuery query,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await mediator.Send(query, cancellationToken);
            return Results.Ok(result);
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> GetLoanSchedule(
        Guid loanId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetLoanScheduleQuery(loanId);
        try
        {
            var result = await mediator.Send(query, cancellationToken);
            return Results.Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { error = ex.Message });
        }
    }
}
