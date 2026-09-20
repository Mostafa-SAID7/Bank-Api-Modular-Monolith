using FluentValidation;
using Bank.CoreBanking.Presentation.Dtos;
using Microsoft.AspNetCore.Http;

namespace Bank.CoreBanking.Presentation.Extensions;

/// <summary>
/// Extension methods for validation in minimal APIs
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Validates a request object and returns an error result if validation fails
    /// </summary>
    /// <typeparam name="T">Type of object to validate</typeparam>
    /// <param name="validator">Validator instance</param>
    /// <param name="obj">Object to validate</param>
    /// <returns>IResult with error response if invalid, null if valid</returns>
    public static IResult? ValidateAndReturnError<T>(
        this IValidator<T> validator,
        T obj) where T : class
    {
        var result = validator.Validate(obj);
        if (!result.IsValid)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.ErrorMessage));
            return Results.BadRequest(new ErrorResponse(errors));
        }

        return null;
    }

    /// <summary>
    /// Validates pagination parameters and returns an error result if validation fails
    /// </summary>
    /// <param name="paginationParams">Pagination parameters to validate</param>
    /// <returns>IResult with error response if invalid, null if valid</returns>
    public static IResult? ValidatePaginationParams(PaginationParams paginationParams)
    {
        if (paginationParams.PageNumber < 1 || paginationParams.PageSize < 1 || paginationParams.PageSize > 100)
        {
            return Results.BadRequest(new ErrorResponse(
                "Invalid pagination parameters. PageNumber >= 1, 1 <= PageSize <= 100"
            ));
        }

        return null;
    }
}
