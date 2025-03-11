using Ardalis.Result;
using FastEndpoints;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationById;
using KeepItUp.MagJob.Identity.Web.Services;
using MediatR;

namespace KeepItUp.MagJob.Identity.Web.Endpoints.Organizations;

/// <summary>
/// Endpoint do pobierania organizacji po identyfikatorze.
/// </summary>
/// <remarks>
/// Pobiera organizację o podanym identyfikatorze.
/// </remarks>
public class GetOrganizationEndpoint(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<GetOrganizationRequest, GetOrganizationResponse>
{
    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Get("api/organizations/{id}");
        AllowAnonymous(); // Tymczasowo, do czasu naprawienia autoryzacji
        Description(b => b
            .WithName("GetOrganization")
            .Produces<GetOrganizationResponse>(200)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s => {
            s.Summary = "Pobiera organizację";
            s.Description = "Pobiera organizację o podanym identyfikatorze";
        });
    }

    /// <summary>
    /// Obsługuje żądanie GET /api/organizations/{id}.
    /// </summary>
    /// <param name="req">Żądanie.</param>
    /// <param name="ct">Token anulowania.</param>
    /// <returns>Odpowiedź z danymi organizacji.</returns>
    public override async Task HandleAsync(GetOrganizationRequest req, CancellationToken ct)
    {
        try
        {
            // Pobierz ID użytkownika z CurrentUserAccessor
            var userGuid = currentUserAccessor.GetRequiredCurrentUserId();

            var query = new GetOrganizationByIdQuery
            {
                OrganizationId = req.Id,
                UserId = userGuid
            };

            var result = await mediator.Send(query, ct);

            if (result.Status == ResultStatus.NotFound)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            if (result.Status == ResultStatus.Forbidden)
            {
                await SendForbiddenAsync(ct);
                return;
            }

            if (result.Status == ResultStatus.Error)
            {
                await SendErrorsAsync(500, ct);
                return;
            }

            Response = new GetOrganizationResponse
            {
                Id = result.Value.Id,
                Name = result.Value.Name,
                Description = result.Value.Description,
                OwnerId = result.Value.OwnerId,
                IsOwner = result.Value.OwnerId == userGuid,
                MemberCount = 0 // Tymczasowo ustawiamy na 0
            };

            await SendOkAsync(Response, ct);
        }
        catch (UnauthorizedAccessException)
        {
            AddError("Nie można zidentyfikować użytkownika");
            await SendErrorsAsync(401, ct);
        }
    }
} 
