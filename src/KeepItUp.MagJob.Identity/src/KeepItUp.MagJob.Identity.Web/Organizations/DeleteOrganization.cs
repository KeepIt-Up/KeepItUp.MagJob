using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.DeactivateOrganization;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint to delete an organization.
/// </summary>
/// <remarks>
/// Deactivates an organization with the given identifier.
/// </remarks>
public class DeleteOrganization(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : BaseEndpoint<DeleteOrganizationRequest, EmptyResponse>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    protected override void ConfigureEndpoint()
    {
        Delete(DeleteOrganizationRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Deactivates an organization";
            s.Description = "Deactivates an organization with the given identifier";
        });
    }

    /// <summary>
    /// Handles the DELETE /api/organizations/{id} request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Empty response in case of success.</returns>
    protected override async Task<EmptyResponse> HandleEndpointAsync(DeleteOrganizationRequest req, CancellationToken ct)
    {
        var userGuid = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new DeactivateOrganizationCommand
        {
            Id = req.Id,
            UserId = userGuid
        };

        return await mediator.Send(command, ct);
    }
}
