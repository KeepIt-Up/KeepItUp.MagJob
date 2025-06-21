using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetRolesByOrganizationId;
using KeepItUp.MagJob.Identity.Web.Services;


namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint do pobierania ról organizacji.
/// </summary>
/// <remarks>
/// Pobiera wszystkie role przypisane do organizacji o podanym identyfikatorze.
/// </remarks>
public class GetOrganizationRoles(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<GetOrganizationRolesRequest, PaginationResult<RoleDto>>
{
    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Get(GetOrganizationRolesRequest.Route);
        AllowAnonymous(); // Tymczasowo, do czasu naprawienia autoryzacji
        Description(b => b
            .WithName("GetOrganizationRoles")
            .Produces<GetOrganizationRolesResponse>(200)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s =>
        {
            s.Summary = "Pobiera role organizacji";
            s.Description = "Pobiera wszystkie role przypisane do organizacji o podanym identyfikatorze";
            s.ExampleRequest = new GetOrganizationRolesRequest { OrganizationId = Guid.NewGuid() };
        });
    }

    /// <summary>
    /// Obsługuje żądanie GET /api/organizations/{organizationId}/roles.
    /// </summary>
    /// <param name="req">Żądanie.</param>
    /// <param name="ct">Token anulowania.</param>
    /// <returns>Odpowiedź z listą ról organizacji.</returns>
    public override async Task HandleAsync(GetOrganizationRolesRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var query = new GetRolesByOrganizationIdQuery
        {
            OrganizationId = req.OrganizationId,
            UserId = userId
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

        Response = result.Value;

        await SendOkAsync(Response, ct);
    }
}
