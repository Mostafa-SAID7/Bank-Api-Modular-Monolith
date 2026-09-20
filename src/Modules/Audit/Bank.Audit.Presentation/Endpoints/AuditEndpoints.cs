namespace Bank.Audit.Presentation.Endpoints;

using Bank.Audit.Application.Commands;
using Bank.Audit.Application.DTOs;
using Bank.Audit.Application.Queries;
using Bank.Audit.Domain.Enums;
using Bank.Audit.Presentation.Requests;
using Bank.Audit.Presentation.Responses;

public static class AuditEndpoints
{
    public static void MapAuditEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/audit")
            .WithName("Audit")
            .WithOpenApi();

        group.MapPost("/logs", CreateAuditLog)
            .WithName("CreateAuditLog")
            .WithOpenApi()
            .Produces<CreateAuditLogResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapGet("/logs/{id:guid}", GetAuditLog)
            .WithName("GetAuditLog")
            .WithOpenApi()
            .Produces<GetAuditLogResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/logs/user/{userId}", GetAuditsByUser)
            .WithName("GetAuditsByUser")
            .WithOpenApi()
            .Produces<GetAuditLogsResponse>(StatusCodes.Status200OK);

        group.MapGet("/logs/range", GetAuditsByDateRange)
            .WithName("GetAuditsByDateRange")
            .WithOpenApi()
            .Produces<GetAuditLogsResponse>(StatusCodes.Status200OK);

        group.MapPost("/entries", RecordAuditEntry)
            .WithName("RecordAuditEntry")
            .WithOpenApi()
            .Produces<RecordAuditEntryResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPost("/trails", GenerateAuditTrail)
            .WithName("GenerateAuditTrail")
            .WithOpenApi()
            .Produces<GenerateAuditTrailResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPost("/logs/{id:guid}/export", ExportAuditLog)
            .WithName("ExportAuditLog")
            .WithOpenApi()
            .Produces<ExportAuditLogResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapGet("/statistics", GetAuditStatistics)
            .WithName("GetAuditStatistics")
            .WithOpenApi()
            .Produces<AuditStatisticsResponse>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> CreateAuditLog(
        CreateAuditLogRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<AuditEventType>(request.EventType, true, out var eventType))
            return Results.BadRequest(new ErrorResponse("Invalid event type"));

        if (!Enum.TryParse<ResourceType>(request.ResourceType, true, out var resourceType))
            return Results.BadRequest(new ErrorResponse("Invalid resource type"));

        var command = new CreateAuditLogCommand(
            request.UserId,
            eventType,
            resourceType,
            request.ResourceId,
            request.Action,
            request.Timestamp,
            request.IpAddress,
            request.UserAgent,
            request.Details);

        var result = await sender.Send(command, cancellationToken);
        return Results.Created($"/api/audit/logs/{result}", new CreateAuditLogResponse(result));
    }

    private static async Task<IResult> GetAuditLog(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetAuditLogQuery(id);
        var result = await sender.Send(query, cancellationToken);

        if (result is null)
            return Results.NotFound();

        var response = new GetAuditLogResponse(
            result.Id,
            result.UserId,
            result.EventType.ToString(),
            result.ResourceType.ToString(),
            result.ResourceId,
            result.Action,
            result.Timestamp,
            result.IpAddress,
            result.UserAgent,
            result.Status.ToString(),
            result.Details);

        return Results.Ok(response);
    }

    private static async Task<IResult> GetAuditsByUser(
        string userId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetAuditsByUserQuery(userId);
        var result = await sender.Send(query, cancellationToken);
        var response = new GetAuditLogsResponse(
            result.Select(a => new GetAuditLogResponse(
                a.Id, a.UserId, a.EventType.ToString(), a.ResourceType.ToString(),
                a.ResourceId, a.Action, a.Timestamp, a.IpAddress, a.UserAgent,
                a.Status.ToString(), a.Details)),
            result.Count());
        return Results.Ok(response);
    }

    private static async Task<IResult> GetAuditsByDateRange(
        DateTime startDate,
        DateTime endDate,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetAuditsByDateRangeQuery(startDate, endDate);
        var result = await sender.Send(query, cancellationToken);
        var response = new GetAuditLogsResponse(
            result.Select(a => new GetAuditLogResponse(
                a.Id, a.UserId, a.EventType.ToString(), a.ResourceType.ToString(),
                a.ResourceId, a.Action, a.Timestamp, a.IpAddress, a.UserAgent,
                a.Status.ToString(), a.Details)),
            result.Count());
        return Results.Ok(response);
    }

    private static async Task<IResult> RecordAuditEntry(
        RecordAuditEntryRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<ActionType>(request.ActionType, true, out var actionType))
            return Results.BadRequest(new ErrorResponse("Invalid action type"));

        if (!Enum.TryParse<EntityChangeType>(request.ChangeType, true, out var changeType))
            return Results.BadRequest(new ErrorResponse("Invalid change type"));

        var command = new RecordAuditEntryCommand(
            request.AuditLogId,
            request.EntityName,
            actionType,
            request.OldValues,
            request.NewValues,
            changeType);

        var result = await sender.Send(command, cancellationToken);
        return Results.Created($"/api/audit/entries/{result}", new RecordAuditEntryResponse(result));
    }

    private static async Task<IResult> GenerateAuditTrail(
        GenerateAuditTrailRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GenerateAuditTrailCommand(
            request.TrailName,
            request.StartDate,
            request.EndDate,
            request.TotalEntries,
            request.FilePath,
            request.FileFormat);

        var result = await sender.Send(command, cancellationToken);
        return Results.Created($"/api/audit/trails/{result}", new GenerateAuditTrailResponse(result));
    }

    private static async Task<IResult> ExportAuditLog(
        Guid id,
        [FromQuery] string format,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ExportAuditLogCommand(id, format);
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(new ExportAuditLogResponse(result));
    }

    private static async Task<IResult> GetAuditStatistics(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetAuditStatisticsQuery();
        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(new AuditStatisticsResponse(
            result.TotalLogs,
            result.TotalEntriesRecorded,
            result.OldestLogDate,
            result.NewestLogDate,
            result.ActiveLogs,
            result.ArchivedLogs));
    }
}

public record ErrorResponse(string Message);
