namespace Bank.Cards.Presentation.Endpoints;

using Microsoft.AspNetCore.Http;

/// <summary>
/// Cards API endpoints for card management and operations
/// Provides operations: issue, activate, block, unblock, close, replace cards
/// And read operations: get card details, customer cards, validate transactions
/// </summary>
public static class CardsEndpoints
{
    public static void MapCardsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/cards")
            .WithName("Cards");

        // Card lifecycle management endpoints
        group.MapPost("/issue", IssueCard)
            .WithName("IssueCard")
            .Produces<CardResponseDto>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/activate", ActivateCard)
            .WithName("ActivateCard")
            
            .Produces<CardResponseDto>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/block", BlockCard)
            .WithName("BlockCard")
            
            .Produces<CardResponseDto>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/{cardId}/unblock", UnblockCard)
            .WithName("UnblockCard")
            
            .Produces<CardResponseDto>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/{cardId}/close", CloseCard)
            .WithName("CloseCard")
            
            .Produces<CardResponseDto>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/{cardId}/replace", ReplaceCard)
            .WithName("ReplaceCard")
            
            .Produces<CardResponseDto>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        // Card security management
        group.MapPost("/change-pin", ChangePin)
            .WithName("ChangePin")
            
            .Produces<CardResponseDto>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/update-limits", UpdateLimits)
            .WithName("UpdateLimits")
            
            .Produces<CardResponseDto>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        // Card query endpoints
        group.MapGet("/{cardId}", GetCard)
            .WithName("GetCard")
            
            .Produces<CardResponseDto>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapGet("/customer/{customerId}", GetCustomerCards)
            .WithName("GetCustomerCards")
            
            .Produces<IEnumerable<CardResponseDto>>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> IssueCard(
        IssueCardRequest request,
        IValidator<IssueCardRequest> validator,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Results.BadRequest(new ErrorResponse(
                "Validation failed",
                400,
                string.Join("; ", validation.Errors.Select(e => e.ErrorMessage))));

        try
        {
            var command = new IssueCardCommand(
                request.CustomerId,
                request.LinkedAccountId,
                request.CardProductId,
                request.HolderName);

            var card = await mediator.Send(command, cancellationToken);
            return Results.Created($"/api/v1/cards/{card.Id}", MapToResponse(card));
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new ErrorResponse(ex.Message, 400));
        }
    }

    private static async Task<IResult> ActivateCard(
        ActivateCardRequest request,
        IValidator<ActivateCardRequest> validator,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Results.BadRequest(new ErrorResponse(
                "Validation failed",
                400,
                string.Join("; ", validation.Errors.Select(e => e.ErrorMessage))));

        try
        {
            var command = new ActivateCardCommand(request.CardId, request.Pin);
            var card = await mediator.Send(command, cancellationToken);
            return Results.Ok(MapToResponse(card));
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new ErrorResponse(ex.Message, 400));
        }
    }

    private static async Task<IResult> BlockCard(
        BlockCardRequest request,
        IValidator<BlockCardRequest> validator,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Results.BadRequest(new ErrorResponse(
                "Validation failed",
                400,
                string.Join("; ", validation.Errors.Select(e => e.ErrorMessage))));

        try
        {
            var command = new BlockCardCommand(request.CardId, request.Reason, request.Description);
            var card = await mediator.Send(command, cancellationToken);
            return Results.Ok(MapToResponse(card));
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new ErrorResponse(ex.Message, 400));
        }
    }

    private static async Task<IResult> UnblockCard(
        Guid cardId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new UnblockCardCommand(cardId);
            var card = await mediator.Send(command, cancellationToken);
            return Results.Ok(MapToResponse(card));
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new ErrorResponse(ex.Message, 400));
        }
    }

    private static async Task<IResult> CloseCard(
        Guid cardId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CloseCardCommand(cardId);
            var card = await mediator.Send(command, cancellationToken);
            return Results.Ok(MapToResponse(card));
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new ErrorResponse(ex.Message, 400));
        }
    }

    private static async Task<IResult> ReplaceCard(
        Guid cardId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new ReplaceCardCommand(cardId);
            var card = await mediator.Send(command, cancellationToken);
            return Results.Ok(MapToResponse(card));
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new ErrorResponse(ex.Message, 400));
        }
    }

    private static async Task<IResult> ChangePin(
        ChangePinRequest request,
        IValidator<ChangePinRequest> validator,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Results.BadRequest(new ErrorResponse(
                "Validation failed",
                400,
                string.Join("; ", validation.Errors.Select(e => e.ErrorMessage))));

        try
        {
            var command = new ChangePinCommand(
                request.CardId,
                request.CurrentPin,
                request.NewPin);

            var card = await mediator.Send(command, cancellationToken);
            return Results.Ok(MapToResponse(card));
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new ErrorResponse(ex.Message, 400));
        }
    }

    private static async Task<IResult> UpdateLimits(
        UpdateCardLimitsRequest request,
        IValidator<UpdateCardLimitsRequest> validator,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Results.BadRequest(new ErrorResponse(
                "Validation failed",
                400,
                string.Join("; ", validation.Errors.Select(e => e.ErrorMessage))));

        try
        {
            var command = new UpdateCardLimitsCommand(
                request.CardId,
                request.DailyWithdrawalLimit,
                request.DailyTransactionLimit,
                request.DailyForeignTransactionLimit);

            var card = await mediator.Send(command, cancellationToken);
            return Results.Ok(MapToResponse(card));
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new ErrorResponse(ex.Message, 400));
        }
    }

    private static async Task<IResult> GetCard(
        Guid cardId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetCardQuery(cardId);
            var cardDto = await mediator.Send(query, cancellationToken);
            
            var response = new CardResponseDto(
                cardDto.Id,
                cardDto.CustomerId,
                cardDto.MaskedPan,
                cardDto.CardType,
                cardDto.CardBrand,
                cardDto.Status,
                cardDto.HolderName,
                cardDto.IssuedDateUtc,
                cardDto.ExpiryDateUtc,
                cardDto.ActivatedDateUtc,
                cardDto.BlockedDateUtc,
                cardDto.ClosedDateUtc,
                cardDto.DailyWithdrawalLimit,
                cardDto.DailyTransactionLimit,
                cardDto.InternationalEnabled,
                cardDto.OnlineTransactionsEnabled,
                cardDto.ContactlessEnabled);

            return Results.Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Results.NotFound(new ErrorResponse(ex.Message, 404));
        }
    }

    private static async Task<IResult> GetCustomerCards(
        Guid customerId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetCustomerCardsQuery(customerId);
            var cardsDto = await mediator.Send(query, cancellationToken);
            
            var responses = cardsDto.Select(c => new CardResponseDto(
                c.Id,
                c.CustomerId,
                c.MaskedPan,
                c.CardType,
                c.CardBrand,
                c.Status,
                c.HolderName,
                c.IssuedDateUtc,
                c.ExpiryDateUtc,
                c.ActivatedDateUtc,
                c.BlockedDateUtc,
                c.ClosedDateUtc,
                c.DailyWithdrawalLimit,
                c.DailyTransactionLimit,
                c.InternationalEnabled,
                c.OnlineTransactionsEnabled,
                c.ContactlessEnabled));

            return Results.Ok(responses);
        }
        catch (InvalidOperationException ex)
        {
            return Results.NotFound(new ErrorResponse(ex.Message, 404));
        }
    }

    private static CardResponseDto MapToResponse(Card card)
    {
        return new CardResponseDto(
            card.Id,
            card.CustomerId,
            card.MaskedPan,
            card.CardType,
            card.CardBrand,
            card.Status,
            card.HolderName,
            card.IssuedDateUtc,
            card.ExpiryDateUtc,
            card.ActivatedDateUtc,
            card.BlockedDateUtc,
            card.ClosedDateUtc,
            card.DailyWithdrawalLimit,
            card.DailyTransactionLimit,
            card.InternationalEnabled,
            card.OnlineTransactionsEnabled,
            card.ContactlessEnabled);
    }
}

