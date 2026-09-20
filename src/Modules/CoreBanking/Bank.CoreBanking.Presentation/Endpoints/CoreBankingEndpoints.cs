using Bank.CoreBanking.Application;
using Bank.CoreBanking.Application.Handlers;
using Bank.CoreBanking.Application.Queries;
using Bank.CoreBanking.Presentation.Dtos;
using Bank.CoreBanking.Presentation.Extensions;
using Bank.CoreBanking.Presentation.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bank.CoreBanking.Presentation.Endpoints;

/// <summary>
/// Core Banking endpoints for account and transaction management
/// Provides read operations for accounts and transactions
/// Write operations (posting) are handled internally via ICoreBankingPostingContract
/// </summary>
public static class CoreBankingEndpoints
{
    public static void MapCoreBankingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/core-banking")
            .WithName("CoreBanking")
            .WithOpenApi();

        // Account endpoints
        group.MapPost("/accounts", CreateAccount)
            .WithName("CreateAccount")
            .WithOpenApi()
            .Produces<AccountResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status409Conflict);

        group.MapGet("/accounts/{id}", GetAccount)
            .WithName("GetAccount")
            .WithOpenApi()
            .Produces<AccountResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapGet("/accounts/customer/{customerId}", GetCustomerAccounts)
            .WithName("GetCustomerAccounts")
            .WithOpenApi()
            .Produces<PaginatedResponse<AccountResponse>>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        // Transaction endpoints
        group.MapGet("/transactions/{id}", GetTransaction)
            .WithName("GetTransaction")
            .WithOpenApi()
            .Produces<TransactionResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapGet("/accounts/{accountId}/transactions", GetAccountTransactions)
            .WithName("GetAccountTransactions")
            .WithOpenApi()
            .Produces<PaginatedResponse<TransactionResponse>>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> CreateAccount(
        CreateAccountRequest request,
        IValidator<CreateAccountRequest> validator,
        IAccountRepository accountRepository,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate request using FluentValidation
            var validationError = validator.ValidateAndReturnError(request);
            if (validationError != null)
                return validationError;

            // Generate unique account number (format: ACC + 12 hex digits = 15 chars)
            string accountNumber;
            Account? existing;
            
            // Retry up to 3 times in case of collision (extremely unlikely)
            for (int attempt = 0; attempt < 3; attempt++)
            {
                accountNumber = $"ACC{Guid.NewGuid():N}".Substring(0, 15).ToUpper();
                existing = await accountRepository.GetByAccountNumberAsync(accountNumber, cancellationToken);
                if (existing == null)
                    break;
            }

            // If all 3 attempts resulted in collisions (astronomically unlikely), return conflict
            existing = await accountRepository.GetByAccountNumberAsync(accountNumber, cancellationToken);
            if (existing != null)
                return Results.Conflict(new ErrorResponse("Unable to generate unique account number. Please try again."));

            // Create account using domain factory method
            var account = Account.Create(
                accountNumber,
                request.AccountHolderName,
                request.CustomerId,
                currency: request.Currency,
                type: (Bank.CoreBanking.Domain.Enums.AccountType)request.Type,
                initialBalance: 0m
            );

            // Persist account to database
            await accountRepository.AddAsync(account, cancellationToken);
            await accountRepository.SaveChangesAsync(cancellationToken);

            // Return created response with Location header
            var response = new AccountResponse(
                account.Id,
                account.AccountNumber,
                account.AccountHolderName,
                account.CustomerId,
                account.Balance,
                account.Currency,
                (int)account.Status,
                (int)account.Type,
                account.OpenedAtUtc,
                account.LastActivityDate
            );

            return Results.Created($"/api/v1/core-banking/accounts/{response.Id}", response);
        }
        catch (Exception ex)
        {
            return Results.StatusCode(500);
        }
    }

    private static async Task<IResult> GetAccount(
        Guid id,
        IAccountRepository accountRepository,
        CancellationToken cancellationToken)
    {
        try
        {
            if (id == Guid.Empty)
                return Results.BadRequest(new ErrorResponse("Invalid account ID"));

            var account = await accountRepository.GetByIdAsync(id, cancellationToken);
            if (account == null)
                return Results.NotFound(new ErrorResponse("Account not found"));

            var response = new AccountResponse(
                account.Id,
                account.AccountNumber,
                account.AccountHolderName,
                account.CustomerId,
                account.Balance,
                account.Currency,
                (int)account.Status,
                (int)account.Type,
                account.OpenedAtUtc,
                account.LastActivityDate
            );

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.StatusCode(500);
        }
    }

    private static async Task<IResult> GetCustomerAccounts(
        Guid customerId,
        [AsParameters] PaginationParams paginationParams,
        IValidator<PaginationParams> paginationValidator,
        IAccountRepository accountRepository,
        CancellationToken cancellationToken)
    {
        try
        {
            if (customerId == Guid.Empty)
                return Results.BadRequest(new ErrorResponse("Invalid customer ID"));

            // Validate pagination
            var paginationError = paginationValidator.ValidateAndReturnError(paginationParams);
            if (paginationError != null)
                return paginationError;

            var accounts = await accountRepository.GetByCustomerIdAsync(customerId, cancellationToken);

            // Apply pagination
            var totalCount = accounts.Count;
            var pagedAccounts = accounts
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .Select(a => new AccountResponse(
                    a.Id,
                    a.AccountNumber,
                    a.AccountHolderName,
                    a.CustomerId,
                    a.Balance,
                    a.Currency,
                    (int)a.Status,
                    (int)a.Type,
                    a.OpenedAtUtc,
                    a.LastActivityDate
                ))
                .ToList();

            var response = new PaginatedResponse<AccountResponse>(
                pagedAccounts,
                paginationParams.PageNumber,
                paginationParams.PageSize,
                totalCount
            );

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.StatusCode(500);
        }
    }

    private static async Task<IResult> GetTransaction(
        Guid id,
        ITransactionRepository transactionRepository,
        CancellationToken cancellationToken)
    {
        try
        {
            if (id == Guid.Empty)
                return Results.BadRequest(new ErrorResponse("Invalid transaction ID"));

            var transaction = await transactionRepository.GetByIdAsync(id, cancellationToken);
            if (transaction == null)
                return Results.NotFound(new ErrorResponse("Transaction not found"));

            var response = new TransactionResponse(
                transaction.Id,
                transaction.FromAccountId,
                transaction.ToAccountId,
                transaction.Amount,
                transaction.Currency,
                (int)transaction.Status,
                (int)transaction.Type,
                transaction.Reference,
                transaction.Description,
                transaction.CreatedAtUtc,
                transaction.CompletedAtUtc,
                transaction.FailureReason
            );

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.StatusCode(500);
        }
    }

    private static async Task<IResult> GetAccountTransactions(
        Guid accountId,
        [AsParameters] PaginationParams paginationParams,
        IValidator<PaginationParams> paginationValidator,
        ITransactionRepository transactionRepository,
        CancellationToken cancellationToken)
    {
        try
        {
            if (accountId == Guid.Empty)
                return Results.BadRequest(new ErrorResponse("Invalid account ID"));

            // Validate pagination
            var paginationError = paginationValidator.ValidateAndReturnError(paginationParams);
            if (paginationError != null)
                return paginationError;

            var transactions = await transactionRepository.GetByAccountAsync(accountId, cancellationToken);

            // Apply pagination
            var totalCount = transactions.Count;
            var pagedTransactions = transactions
                .OrderByDescending(t => t.InitiatedAtUtc) // Most recent first
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .Select(t => new TransactionResponse(
                    t.Id,
                    t.FromAccountId,
                    t.ToAccountId,
                    t.Amount,
                    t.Currency,
                    (int)t.Status,
                    (int)t.Type,
                    t.Reference,
                    t.Description,
                    t.CreatedAtUtc,
                    t.CompletedAtUtc,
                    t.FailureReason
                ))
                .ToList();

            var response = new PaginatedResponse<TransactionResponse>(
                pagedTransactions,
                paginationParams.PageNumber,
                paginationParams.PageSize,
                totalCount
            );

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.StatusCode(500);
        }
    }
}
