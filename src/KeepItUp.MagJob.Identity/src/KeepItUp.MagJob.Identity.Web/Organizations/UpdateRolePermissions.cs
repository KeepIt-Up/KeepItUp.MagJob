using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateRolePermissions;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint to update the permissions of a role in an organization.
/// </summary>
/// <remarks>
/// Updates the permissions of a role in an organization with the given identifier.
/// </remarks>
public class UpdateRolePermissions(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : BaseEndpoint<UpdateRolePermissionsRequest, EmptyResponse>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    protected override void ConfigureEndpoint()
    {
        Put(UpdateRolePermissionsRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Updates the permissions of a role in an organization";
            s.Description = "Updates the permissions of a role in an organization with the given identifier";
            s.ExampleRequest = new UpdateRolePermissionsRequest
            {
                OrganizationId = Guid.NewGuid(),
                RoleId = Guid.NewGuid(),
                Permissions = new List<string> { "organization.create", "organization.update" }
            };
        });
    }

    /// <summary>
    /// Handles the PUT /api/organizations/{organizationId}/roles/{roleId}/permissions request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Empty response in case of success.</returns>
    protected override async Task<EmptyResponse> HandleEndpointAsync(UpdateRolePermissionsRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new UpdateRolePermissionsCommand
        {
            OrganizationId = req.OrganizationId,
            RoleId = req.RoleId,
            Permissions = req.Permissions,
            UserId = userId
        };

        return await mediator.Send(command, ct);
    }
}
