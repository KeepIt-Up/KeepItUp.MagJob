using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.DeleteRole;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint to delete a role from an organization.
/// </summary>
/// <remarks>
/// Deletes a role from an organization with the given identifier.
/// </remarks>
public class DeleteRole(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : BaseEndpoint<DeleteRoleRequest, EmptyResponse>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    protected override void ConfigureEndpoint()
    {
        Delete(DeleteRoleRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Deletes a role from an organization";
            s.Description = "Deletes a role from an organization with the given identifier";
            s.ExampleRequest = new DeleteRoleRequest { OrganizationId = Guid.NewGuid(), RoleId = Guid.NewGuid() };
        });
    }

    /// <summary>
    /// Handles the DELETE /api/organizations/{organizationId}/roles/{roleId} request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Empty response in case of success.</returns>
    protected override async Task<EmptyResponse> HandleEndpointAsync(DeleteRoleRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new DeleteRoleCommand
        {
            OrganizationId = req.OrganizationId,
            RoleId = req.RoleId,
            UserId = userId
        };

        return await mediator.Send(command, ct);
    }
}
