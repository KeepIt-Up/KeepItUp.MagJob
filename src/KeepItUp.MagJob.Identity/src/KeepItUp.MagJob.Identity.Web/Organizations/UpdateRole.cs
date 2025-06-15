using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateRole;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint to update a role in an organization.
/// </summary>
/// <remarks>
/// Updates a role in an organization with the given identifier.
/// </remarks>
public class UpdateRole(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : BaseEndpoint<UpdateRoleRequest, EmptyResponse>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    protected override void ConfigureEndpoint()
    {
        Put(UpdateRoleRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Aktualizuje rolę w organizacji";
            s.Description = "Aktualizuje rolę w organizacji o podanym identyfikatorze";
            s.ExampleRequest = new UpdateRoleRequest
            {
                OrganizationId = Guid.NewGuid(),
                RoleId = Guid.NewGuid(),
                Name = "Administrator",
                Description = "Rola administratora organizacji",
                Color = "#FF0000"
            };
        });
    }

    /// <summary>
    /// Handles the PUT /api/organizations/{organizationId}/roles/{roleId} request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Empty response in case of success.</returns>
    protected override async Task<EmptyResponse> HandleEndpointAsync(UpdateRoleRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new UpdateRoleCommand
        {
            OrganizationId = req.OrganizationId,
            RoleId = req.RoleId,
            Name = req.Name,
            Description = req.Description,
            Color = req.Color,
            UserId = userId
        };

        return await mediator.Send(command, ct);
    }
}
