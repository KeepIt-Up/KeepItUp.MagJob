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
    : BaseEndpoint<GetOrganizationRolesRequest, PaginationResult<RoleDto>>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    protected override void ConfigureEndpoint()
    {
        Get(GetOrganizationRolesRequest.Route);
        AllowAnonymous();
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
    protected override async Task<PaginationResult<RoleDto>> HandleEndpointAsync(GetOrganizationRolesRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var query = new GetRolesByOrganizationIdQuery
        {
            OrganizationId = req.OrganizationId,
            UserId = userId
        };

        return await mediator.Send(query, ct);
    }
}
