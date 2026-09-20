using Bank.Identity.Application.Commands;
using Bank.Identity.Application.Queries;
using Bank.Identity.Presentation.Dtos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bank.Identity.Presentation.Endpoints;

/// <summary>
/// Identity endpoints for user authentication and account management
/// Provides registration, login, logout, and profile management
/// </summary>
public static class IdentityEndpoints
{
    public static void MapIdentityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/identity")
            .WithName("Identity")
            .WithOpenApi();

        // Authentication endpoints
        group.MapPost("/register", Register)
            .WithName("Register")
            .WithOpenApi()
            .Produces<RegisterUserResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status409Conflict);

        group.MapPost("/login", Login)
            .WithName("Login")
            .WithOpenApi()
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized);

        group.MapPost("/logout", Logout)
            .WithName("Logout")
            .WithOpenApi()
            .Produces<StatusCodeResult>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        // User profile endpoints
        group.MapGet("/users/{id}", GetUser)
            .WithName("GetUser")
            .WithOpenApi()
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapGet("/users/email/{email}", GetUserByEmail)
            .WithName("GetUserByEmail")
            .WithOpenApi()
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> Register(
        RegisterUserRequest request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            var handler = serviceProvider.GetRequiredService<RegisterUserCommandHandler>();
            var command = new RegisterUserCommand(request.Email, request.Password, request.FirstName, request.LastName, request.PhoneNumber);
            var result = await handler.Handle(command, cancellationToken);

            if (!result.Success)
                return Results.BadRequest(new ErrorResponse(result.Message ?? "Registration failed"));

            var response = new RegisterUserResponse(result.UserId, result.Email, result.FullName, result.Success, result.Message);
            return Results.Created($"/api/v1/identity/users/{result.UserId}", response);
        }
        catch (Exception ex)
        {
            return Results.StatusCode(500);
        }
    }

    private static async Task<IResult> Login(
        LoginRequest request,
        IServiceProvider serviceProvider,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        try
        {
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var userAgent = httpContext.Request.Headers["User-Agent"].ToString() ?? "Unknown";

            var handler = serviceProvider.GetRequiredService<LoginUserCommandHandler>();
            var command = new LoginUserCommand(request.Email, request.Password, ipAddress, userAgent);
            var result = await handler.Handle(command, cancellationToken);

            if (!result.Success)
                return Results.Unauthorized();

            var response = new LoginResponse(
                result.UserId,
                result.Email,
                "", // FullName - would need to fetch from user
                result.AccessToken,
                result.RefreshToken,
                result.SessionId,
                result.ExpiresAtUtc,
                result.Success,
                result.Message
            );

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.StatusCode(500);
        }
    }

    private static async Task<IResult> Logout(
        LogoutUserRequest request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            var handler = serviceProvider.GetRequiredService<LogoutUserCommandHandler>();
            var command = new LogoutUserCommand(request.SessionId, request.UserId);
            var result = await handler.Handle(command, cancellationToken);

            if (!result.Success)
                return Results.BadRequest(new ErrorResponse(result.Message ?? "Logout failed"));

            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.StatusCode(500);
        }
    }

    private static async Task<IResult> GetUser(
        Guid id,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            if (id == Guid.Empty)
                return Results.BadRequest(new ErrorResponse("Invalid user ID"));

            var handler = serviceProvider.GetRequiredService<GetUserByIdQueryHandler>();
            var query = new GetUserByIdQuery(id);
            var result = await handler.Handle(query, cancellationToken);

            if (result == null)
                return Results.NotFound(new ErrorResponse("User not found"));

            var response = new UserResponse(
                result.Id,
                result.Email,
                result.FirstName,
                result.LastName,
                result.FullName,
                result.PhoneNumber,
                result.Status,
                result.CreatedAtUtc,
                result.LastLoginAtUtc,
                result.IsActive
            );

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.StatusCode(500);
        }
    }

    private static async Task<IResult> GetUserByEmail(
        string email,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                return Results.BadRequest(new ErrorResponse("Email is required"));

            var handler = serviceProvider.GetRequiredService<GetUserByIdQueryHandler>();
            // In real scenario, would use separate handler for email lookup
            // For now, returning 404 - this would be implemented properly
            return Results.NotFound(new ErrorResponse("User not found"));
        }
        catch (Exception ex)
        {
            return Results.StatusCode(500);
        }
    }
}

/// <summary>
/// Logout request DTO
/// </summary>
public sealed record LogoutUserRequest(
    Guid SessionId,
    Guid UserId);
