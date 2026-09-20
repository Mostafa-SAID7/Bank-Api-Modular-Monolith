namespace Bank.Statements.Presentation.Endpoints;

/// <summary>
/// REST API endpoints for Statements module
/// Handles statement generation, retrieval, scheduling, and distribution
/// </summary>
public static class StatementsEndpoints
{
    public const string Base = "api/v1/statements";

    public static void MapStatementsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(Base)
            .WithTags("Statements");

        group.MapPost("/generate", GenerateStatement)
            .WithName("GenerateStatement")
            .WithDescription("Generate a new statement for an account");

        group.MapGet("/{statementId}", GetStatement)
            .WithName("GetStatement")
            .WithDescription("Get statement details by ID");

        group.MapGet("/customer/{customerId}", GetCustomerStatements)
            .WithName("GetCustomerStatements")
            .WithDescription("Get all statements for a customer");

        group.MapPost("/{statementId}/send", SendStatement)
            .WithName("SendStatement")
            .WithDescription("Send statement to recipients");

        group.MapPost("/{statementId}/download", DownloadStatement)
            .WithName("DownloadStatement")
            .WithDescription("Download statement file");

        group.MapPost("/{statementId}/recipients", AddRecipient)
            .WithName("AddRecipient")
            .WithDescription("Add recipient for statement delivery");

        var scheduleGroup = app.MapGroup("api/v1/schedules")
            .WithTags("Schedules");

        scheduleGroup.MapPost("", CreateSchedule)
            .WithName("CreateSchedule")
            .WithDescription("Create a statement generation schedule");

        scheduleGroup.MapGet("/{scheduleId}", GetSchedule)
            .WithName("GetSchedule")
            .WithDescription("Get schedule details by ID");
    }

    private static async Task<IResult> GenerateStatement(
        GenerateStatementRequest request,
        IMediator mediator,
        IValidator<GenerateStatementRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        var command = new GenerateStatementCommand(
            request.CustomerId,
            request.AccountId,
            request.PeriodStartDate,
            request.PeriodEndDate,
            request.OpeningBalance);

        try
        {
            var statementId = await mediator.Send(command, cancellationToken);
            return Results.CreatedAtRoute(
                "GetStatement",
                new { statementId },
                new { id = statementId, message = "Statement generated successfully" });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> GetStatement(
        Guid statementId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetStatementQuery(statementId);
        try
        {
            var result = await mediator.Send(query, cancellationToken) as StatementDto;
            if (result == null)
                return Results.NotFound(new { error = "Statement not found" });
            return Results.Ok(MapToResponse(result));
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Forbid();
        }
    }

    private static async Task<IResult> GetCustomerStatements(
        Guid customerId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        IMediator mediator = null!,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCustomerStatementsQuery(customerId);
        try
        {
            var results = await mediator.Send(query, cancellationToken) as IEnumerable<StatementDto>;
            if (results == null || !results.Any())
                return Results.Ok(new List<StatementResponse>());
            return Results.Ok(results.Select(MapToResponse).ToList());
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> SendStatement(
        Guid statementId,
        SendStatementRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new SendStatementCommand(statementId);

        try
        {
            await mediator.Send(command, cancellationToken);
            return Results.Ok(new { message = "Statement sent successfully" });
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

    private static async Task<IResult> DownloadStatement(
        Guid statementId,
        DownloadStatementRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new DownloadStatementCommand(statementId, request.CustomerId);
        try
        {
            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(new { message = "Download initiated", statementId = result });
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Forbid();
        }
    }

    private static async Task<IResult> AddRecipient(
        Guid statementId,
        AddRecipientRequest request,
        IMediator mediator,
        IValidator<AddRecipientRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        var deliveryMethod = Enum.Parse<DeliveryMethod>(request.DeliveryMethod);

        var command = new AddRecipientCommand(
            statementId,
            request.Email,
            deliveryMethod);

        try
        {
            var recipientId = await mediator.Send(command, cancellationToken);
            return Results.CreatedAtRoute(
                "GetStatement",
                new { statementId },
                new { recipientId, message = "Recipient added successfully" });
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

    private static async Task<IResult> CreateSchedule(
        CreateScheduleRequest request,
        IMediator mediator,
        IValidator<CreateScheduleRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        var period = Enum.Parse<StatementPeriod>(request.Period);
        var format = Enum.Parse<StatementFormat>(request.Format);
        var deliveryMethod = Enum.Parse<DeliveryMethod>(request.DeliveryMethod);

        var command = new CreateScheduleCommand(
            request.CustomerId,
            request.AccountId,
            period,
            format,
            deliveryMethod);

        try
        {
            var scheduleId = await mediator.Send(command, cancellationToken);
            return Results.CreatedAtRoute(
                "GetSchedule",
                new { scheduleId },
                new { id = scheduleId, message = "Schedule created successfully" });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> GetSchedule(
        Guid scheduleId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetAccountScheduleQuery(scheduleId);
        try
        {
            var result = await mediator.Send(query, cancellationToken) as StatementScheduleDto;
            if (result == null)
                return Results.NotFound(new { error = "Schedule not found" });
            return Results.Ok(MapScheduleToResponse(result));
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Forbid();
        }
    }

    private static StatementResponse MapToResponse(StatementDto dto) =>
        new(
            dto.Id,
            dto.CustomerId,
            dto.AccountId,
            dto.StatementDate,
            dto.PeriodStartDate,
            dto.PeriodEndDate,
            dto.OpeningBalance,
            dto.ClosingBalance,
            dto.TotalDebits,
            dto.TotalCredits,
            dto.Status.ToString(),
            dto.Period.ToString(),
            dto.Format.ToString(),
            dto.CreatedAt,
            dto.GeneratedAt,
            dto.SentAt,
            dto.DownloadedAt);

    private static ScheduleResponse MapScheduleToResponse(StatementScheduleDto dto) =>
        new(
            dto.Id,
            dto.CustomerId,
            dto.AccountId,
            dto.Period.ToString(),
            dto.Format.ToString(),
            dto.DeliveryMethod.ToString(),
            dto.IsActive,
            dto.NextGenerationDate,
            dto.LastGeneratedDate,
            dto.CreatedAt);
}

