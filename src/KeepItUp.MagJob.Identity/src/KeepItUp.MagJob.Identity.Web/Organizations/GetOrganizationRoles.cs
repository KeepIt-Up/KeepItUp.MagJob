using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetRolesByOrganizationId;
using KeepItUp.MagJob.Identity.Web.Services;


namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint to get the roles of an organization.
/// </summary>
/// <remarks>
/// Gets all roles assigned to an organization with the given identifier.
/// </remarks>
public class GetOrganizationRoles(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<GetOrganizationRolesRequest, PaginationResult<RoleDto>>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    public override void Configure()
    {
        Get(GetOrganizationRolesRequest.Route);
        AllowAnonymous();
        Description(b => b
            .WithName("GetOrganizationRoles")
            .Produces<GetOrganizationRolesResponse>(200)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s =>
        {
            s.Summary = "Gets the roles of an organization";
            s.Description = "Gets all roles assigned to an organization with the given identifier";
            s.ExampleRequest = new GetOrganizationRolesRequest { OrganizationId = Guid.NewGuid() };
        });
    }

    /// <summary>
    /// Handles the GET /api/organizations/{organizationId}/roles request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Response containing the list of roles of the organization.</returns>
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
