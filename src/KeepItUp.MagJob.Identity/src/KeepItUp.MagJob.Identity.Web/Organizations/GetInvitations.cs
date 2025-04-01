using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationInvitations;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint do pobierania zaproszeń do organizacji.
/// </summary>
/// <remarks>
/// Pobiera wszystkie zaproszenia do organizacji o podanym identyfikatorze.
/// </remarks>
public class GetInvitationsEndpoint(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<GetInvitationsRequest, PaginationResult<InvitationDto>>
{
    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Get(GetInvitationsRequest.Route);
        AllowAnonymous(); // Tymczasowo, do czasu naprawienia autoryzacji
        Description(b => b
            .WithName("GetInvitations")
            .Produces<PaginationResult<InvitationDto>>(200)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s =>
        {
            s.Summary = "Pobiera zaproszenia do organizacji";
            s.Description = "Pobiera wszystkie zaproszenia do organizacji o podanym identyfikatorze";
            s.ExampleRequest = new GetInvitationsRequest
            {
                OrganizationId = Guid.NewGuid(),
                PaginationParameters = PaginationParameters<InvitationDto>.Create()
            };
        });
    }

    /// <summary>
    /// Obsługuje żądanie GET /api/organizations/{id}/invitations.
    /// </summary>
    /// <param name="req">Żądanie.</param>
    /// <param name="ct">Token anulowania.</param>
    /// <returns>Odpowiedź z listą zaproszeń do organizacji z paginacją.</returns>
    public override async Task HandleAsync(GetInvitationsRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var query = new GetOrganizationInvitationsQuery
        {
            OrganizationId = req.OrganizationId,
            UserId = userId,
            PaginationParameters = req.PaginationParameters
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

        await SendOkAsync(result.Value, ct);
    }
}
