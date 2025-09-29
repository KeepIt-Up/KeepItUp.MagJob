using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.RemoveMember;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint to remove a member from an organization.
/// </summary>
/// <remarks>
/// Removes a member from an organization with the given identifier.
/// </remarks>
public class RemoveMember(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : BaseEndpoint<RemoveMemberRequest, EmptyResponse>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    protected override void ConfigureEndpoint()
    {
        Delete(RemoveMemberRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Removes a member from an organization";
            s.Description = "Removes a member from an organization with the given identifier";
            s.ExampleRequest = new RemoveMemberRequest { OrganizationId = Guid.NewGuid(), MemberUserId = Guid.NewGuid() };
        });
    }

    /// <summary>
    /// Handles the DELETE /api/organizations/{organizationId}/members/{memberUserId} request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Empty response in case of success.</returns>
    protected override async Task<EmptyResponse> HandleEndpointAsync(RemoveMemberRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new RemoveMemberCommand
        {
            OrganizationId = req.OrganizationId,
            MemberUserId = req.MemberUserId,
            RequestingUserId = userId
        };

        return await mediator.Send(command, ct);
    }
}
