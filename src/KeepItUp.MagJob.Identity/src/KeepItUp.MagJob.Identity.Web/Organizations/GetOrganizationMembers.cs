using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationMembers;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint do pobierania członków organizacji.
/// </summary>
/// <remarks>
/// Pobiera wszystkich członków organizacji o podanym identyfikatorze.
/// </remarks>
public class GetOrganizationMembers(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<GetOrganizationMembersRequest, PaginationResult<MemberDto>>
{
    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Get(GetOrganizationMembersRequest.Route);
        AllowAnonymous(); // Tymczasowo, do czasu naprawienia autoryzacji
        Description(b => b
            .WithName("GetOrganizationMembers")
            .Produces<PaginationResult<MemberDto>>(200)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s =>
        {
            s.Summary = "Pobiera członków organizacji";
            s.Description = "Pobiera wszystkich członków organizacji o podanym identyfikatorze";
            s.ExampleRequest = new GetOrganizationMembersRequest
            {
                OrganizationId = Guid.NewGuid(),
                PaginationParameters = PaginationParameters<MemberDto>.Create()
            };
        });
    }

    /// <summary>
    /// Obsługuje żądanie GET /api/organizations/{organizationId}/members.
    /// </summary>
    /// <param name="req">Żądanie.</param>
    /// <param name="ct">Token anulowania.</param>
    /// <returns>Odpowiedź z listą członków organizacji z paginacją.</returns>
    public override async Task HandleAsync(GetOrganizationMembersRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var query = new GetOrganizationMembersQuery
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
