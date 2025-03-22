using Ardalis.Result;
using FastEndpoints;
using KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserById;
using MediatR;

namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Endpoint do pobierania użytkownika po identyfikatorze.
/// </summary>
/// <remarks>
/// Pobiera użytkownika o podanym identyfikatorze.
/// </remarks>
public class GetUserById(IMediator mediator)
    : Endpoint<GetUserByIdRequest, GetUserByIdResponse>
{
    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Get(GetUserByIdRequest.Route);
        AllowAnonymous(); // Tymczasowo, do czasu naprawienia autoryzacji
        Description(b => b
            .WithName("GetUser")
            .Produces<GetUserByIdResponse>(200)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s => {
            s.Summary = "Pobiera użytkownika";
            s.Description = "Pobiera użytkownika o podanym identyfikatorze";
            s.ExampleRequest = new GetUserByIdRequest { Id = Guid.NewGuid() };
        });
    }

    /// <summary>
    /// Obsługuje żądanie GET /api/users/{id}.
    /// </summary>
    /// <param name="req">Żądanie.</param>
    /// <param name="ct">Token anulowania.</param>
    /// <returns>Odpowiedź z danymi użytkownika.</returns>
    public override async Task HandleAsync(GetUserByIdRequest req, CancellationToken ct)
    {
        var query = new GetUserByIdQuery
        {
            Id = req.Id
        };

        var result = await mediator.Send(query, ct);

        if (result.Status == ResultStatus.NotFound)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (result.Status == ResultStatus.Error)
        {
            await SendErrorsAsync(500, ct);
            return;
        }

        var response = new GetUserByIdResponse
        {
            Id = result.Value.Id,
            ExternalId = result.Value.ExternalId,
            Email = result.Value.Email,
            FirstName = result.Value.FirstName,
            LastName = result.Value.LastName,
            IsActive = result.Value.IsActive
        };

        await SendOkAsync(response, ct);
    }
} 
