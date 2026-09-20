using Bank.Deposits.Presentation.Dtos;

namespace Bank.Deposits.Presentation.Extensions;

/// <summary>
/// Extension methods for validation
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Validates an object and returns an error response if validation fails
    /// </summary>
    public static IResult? ValidateAndReturnError<T>(
        this IValidator<T> validator,
        T objectToValidate)
    {
        var validationResult = validator.Validate(objectToValidate);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Results.BadRequest(new ErrorResponse(
                Message: "Validation failed",
                Details: errors));
        }

        return null;
    }
}
