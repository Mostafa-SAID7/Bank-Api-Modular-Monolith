using Bank.Deposits.Application.Commands;
using Bank.Deposits.Presentation.Dtos;
using Bank.Deposits.Presentation.Extensions;
using Bank.Deposits.Presentation.Validators;
using MediatR;

namespace Bank.Deposits.Presentation.Endpoints;

/// <summary>
/// Deposits API endpoints for account management and transactions
/// Provides operations: open, deposit, withdraw, freeze, unfreeze, close deposits
/// And read operations: get deposit details, balance, customer deposits
/// </summary>
public static class DepositsEndpoints
{
    public static void MapDepositsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/deposits")
            .WithName("Deposits")
            .WithOpenApi();

        // Deposit account management endpoints
        group.MapPost("/open", OpenDeposit)
            .WithName("OpenDeposit")
            .WithOpenApi()
            .Produces<CreateDepositResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapGet("/{depositId}", GetDeposit)
            .WithName("GetDeposit")
            .WithOpenApi()
            .Produces<DepositResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapGet("/customer/{customerId}", GetCustomerDeposits)
            .WithName("GetCustomerDeposits")
            .WithOpenApi()
            .Produces<PaginatedDepositsResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        // Transaction endpoints
        group.MapPost("/deposit", DepositFunds)
            .WithName("DepositFunds")
            .WithOpenApi()
            .Produces<DepositResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/withdraw", WithdrawFunds)
            .WithName("WithdrawFunds")
            .WithOpenApi()
            .Produces<DepositResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        // Account status management
        group.MapPost("/freeze", FreezeDeposit)
            .WithName("FreezeDeposit")
            .WithOpenApi()
            .Produces<DepositResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/unfreeze", UnfreezeDeposit)
            .WithName("UnfreezeDeposit")
            .WithOpenApi()
            .Produces<DepositResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/close", CloseDeposit)
            .WithName("CloseDeposit")
            .WithOpenApi()
            .Produces<DepositResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        // Interest management
        group.MapPost("/{depositId}/pay-interest", PayInterest)
            .WithName("PayInterest")
            .WithOpenApi()
            .Produces<DepositResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        // Balance query
        group.MapGet("/{depositId}/balance", GetDepositBalance)
            .WithName("GetDepositBalance")
            .WithOpenApi()
            .Produces<DepositBalanceResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> OpenDeposit(
        CreateDepositRequest request,
        IValidator<CreateDepositRequest> validator,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        try
        {
            var validationError = validator.ValidateAndReturnError(request);
            if (validationError != null)
                return validationError;

            var command = new OpenDepositCommand(
                request.CustomerId,
                request.DepositTypeId,
                request.InitialDeposit,
                request.IsFixedDeposit,
                request.TermMonths);

            var result = await mediator.Send(command, cancellationToken);

            var response = new CreateDepositResponse(
                result.Id,
                result.AccountNumber,
                result.CustomerId,
                result.CurrentBalance,
                (int)result.Status,
                result.OpenedAtUtc,
                result.MaturityDateUtc,
                result.IsFixedDeposit);

            return Results.Created($"/api/v1/deposits/{result.Id}", response);
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            return Results.StatusCode(500);
        }
    }

    private static async Task<IResult> GetDeposit(
        Guid depositId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        try
        {
            if (depositId == Guid.Empty)
                return Results.BadRequest(new ErrorResponse("Invalid deposit ID"));

            var query = new GetDepositByIdQuery(depositId);
            var result = await mediator.Send(query, cancellationToken);

            if (result == null)
                return Results.NotFound(new ErrorResponse("Deposit not found"));

            var response = new DepositResponse(
                result.Id,
                result.AccountNumber,
                result.CustomerId,
                result.CurrentBalance,
                result.AccruedInterest,
                result.PaidInterest,
                (int)result.Status,
                result.OpenedAtUtc,
                result.MaturityDateUtc,
                result.ClosedAtUtc,
                result.IsFixedDeposit,
                result.TermMonths);

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.StatusCode(500);
        }
    }

    private static async Task<IResult> GetCustomerDeposits(
        Guid customerId,
        [AsParameters] PaginationParams paginationParams,
        IValidator<PaginationParams> paginationValidator,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        try
        {
            if (customerId == Guid.Empty)
                return Results.BadRequest(new ErrorResponse("Invalid customer ID"));

            var paginationError = paginationValidator.ValidateAndReturnError(paginationParams);
            if (paginationError != null)
                return paginationError;

            var query = new GetCustomerDepositsQuery(customerId, paginationParams.PageNumber, paginationParams.PageSize);
            var result = await mediator.Send(query, cancellationToken);

            var items = result.Items.Select(d => new DepositResponse(
                d.Id,
                d.AccountNumber,
                d.CustomerId,
                d.CurrentBalance,
                d.AccruedInterest,
                d.PaidInterest,
                (int)d.Status,
                d.OpenedAtUtc,
                d.MaturityDateUtc,
                d.ClosedAtUtc,
                d.IsFixedDeposit,
                d.TermMonths)).ToList();

            var response = new PaginatedDepositsResponse(
                items,
                result.TotalCount,
                result.PageNumber,
                result.PageSize);

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.StatusCode(500);
        }
    }

    private static async Task<IResult> DepositFunds(
        DepositTransactionRequest request,
        IValidator<DepositTransactionRequest> validator,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        try
        {
            var validationError = validator.ValidateAndReturnError(request);
            if (validationError != null)
                return validationError;

            var command = new DepositFundsCommand(request.DepositId, request.Amount, request.Description);
            var result = await mediator.Send(command, cancellationToken);

            var response = new DepositResponse(
                result.Id,
                result.AccountNumber,
                result.CustomerId,
                result.CurrentBalance,
                result.AccruedInterest,
                result.PaidInterest,
                (int)result.Status,
                result.OpenedAtUtc,
                result.MaturityDateUtc,
                result.ClosedAtUtc,
                result.IsFixedDeposit,
                result.TermMonths);

            return Results.Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new ErrorResponse(ex.Message));
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound(new ErrorResponse("Deposit not found"));
        }
        catch (Exception ex)
        {
            return Results.StatusCode(500);
        }
    }

    private static async Task<IResult> WithdrawFunds(
        DepositTransactionRequest request,
        IValidator<DepositTransactionRequest> validator,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        try
        {
            var validationError = validator.ValidateAndReturnError(request);
            if (validationError != null)
                return validationError;

            var command = new WithdrawFundsCommand(request.DepositId, request.Amount, request.Description);
            var result = await mediator.Send(command, cancellationToken);

            var response = new DepositResponse(
                result.Id,
                result.AccountNumber,
                result.CustomerId,
                result.CurrentBalance,
                result.AccruedInterest,
                result.PaidInterest,
                (int)result.Status,
                result.OpenedAtUtc,
                result.MaturityDateUtc,
                result.ClosedAtUtc,
                result.IsFixedDeposit,
                result.TermMonths);

            return Results.Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new ErrorResponse(ex.Message));
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound(new ErrorResponse("Deposit not found"));
        }
        catch (Exception ex)
        {
            return Results.StatusCode(500);
        }
    }

    private static async Task<IResult> FreezeDeposit(
        FreezeDepositRequest request,
        IValidator<FreezeDepositRequest> validator,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        try
        {
            var validationError = validator.ValidateAndReturnError(request);
            if (validationError != null)
                return validationError;

            var command = new FreezeDepositCommand(request.DepositId, request.Reason);
            var result = await mediator.Send(command, cancellationToken);

            var response = new DepositResponse(
                result.Id,
                result.AccountNumber,
                result.CustomerId,
                result.CurrentBalance,
                result.AccruedInterest,
                result.PaidInterest,
                (int)result.Status,
                result.OpenedAtUtc,
                result.MaturityDateUtc,
                result.ClosedAtUtc,
                result.IsFixedDeposit,
                result.TermMonths);

            return Results.Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new ErrorResponse(ex.Message));
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound(new ErrorResponse("Deposit not found"));
        }
        catch (Exception ex)
        {
            return Results.StatusCode(500);
        }
    }

    private static async Task<IResult> UnfreezeDeposit(
        Guid depositId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        try
        {
            if (depositId == Guid.Empty)
                return Results.BadRequest(new ErrorResponse("Invalid deposit ID"));

            var command = new UnfreezeDepositCommand(depositId);
            var result = await mediator.Send(command, cancellationToken);

            var response = new DepositResponse(
                result.Id,
                result.AccountNumber,
                result.CustomerId,
                result.CurrentBalance,
                result.AccruedInterest,
                result.PaidInterest,
                (int)result.Status,
                result.OpenedAtUtc,
                result.MaturityDateUtc,
                result.ClosedAtUtc,
                result.IsFixedDeposit,
                result.TermMonths);

            return Results.Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new ErrorResponse(ex.Message));
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound(new ErrorResponse("Deposit not found"));
        }
        catch (Exception ex)
        {
            return Results.StatusCode(500);
        }
    }

    private static async Task<IResult> CloseDeposit(
        CloseDepositRequest request,
        IValidator<CloseDepositRequest> validator,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        try
        {
            var validationError = validator.ValidateAndReturnError(request);
            if (validationError != null)
                return validationError;

            var command = new CloseDepositCommand(request.DepositId, request.Reason);
            var result = await mediator.Send(command, cancellationToken);

            var response = new DepositResponse(
                result.Id,
                result.AccountNumber,
                result.CustomerId,
                result.CurrentBalance,
                result.AccruedInterest,
                result.PaidInterest,
                (int)result.Status,
                result.OpenedAtUtc,
                result.MaturityDateUtc,
                result.ClosedAtUtc,
                result.IsFixedDeposit,
                result.TermMonths);

            return Results.Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new ErrorResponse(ex.Message));
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound(new ErrorResponse("Deposit not found"));
        }
        catch (Exception ex)
        {
            return Results.StatusCode(500);
        }
    }

    private static async Task<IResult> PayInterest(
        Guid depositId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        try
        {
            if (depositId == Guid.Empty)
                return Results.BadRequest(new ErrorResponse("Invalid deposit ID"));

            var command = new PayDepositInterestCommand(depositId);
            var result = await mediator.Send(command, cancellationToken);

            var response = new DepositResponse(
                result.Id,
                result.AccountNumber,
                result.CustomerId,
                result.CurrentBalance,
                result.AccruedInterest,
                result.PaidInterest,
                (int)result.Status,
                result.OpenedAtUtc,
                result.MaturityDateUtc,
                result.ClosedAtUtc,
                result.IsFixedDeposit,
                result.TermMonths);

            return Results.Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new ErrorResponse(ex.Message));
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound(new ErrorResponse("Deposit not found"));
        }
        catch (Exception ex)
        {
            return Results.StatusCode(500);
        }
    }

    private static async Task<IResult> GetDepositBalance(
        Guid depositId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        try
        {
            if (depositId == Guid.Empty)
                return Results.BadRequest(new ErrorResponse("Invalid deposit ID"));

            var query = new GetDepositBalanceQuery(depositId);
            var result = await mediator.Send(query, cancellationToken);

            if (result == null)
                return Results.NotFound(new ErrorResponse("Deposit not found"));

            var response = new DepositBalanceResponse(
                result.DepositId,
                result.AccountNumber,
                result.CurrentBalance,
                result.AccruedInterest,
                result.PaidInterest,
                result.TotalBalance);

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.StatusCode(500);
        }
    }
}
