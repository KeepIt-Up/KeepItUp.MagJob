using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.RevokeRoleFromMember;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint to revoke a role from a member of an organization.
/// </summary>
/// <remarks>
/// Revokes a role from a member of an organization with the given identifier.
/// </remarks>
public class RevokeRoleFromMember(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : BaseEndpoint<RevokeRoleFromMemberRequest, EmptyResponse>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    protected override void ConfigureEndpoint()
    {
        Delete(RevokeRoleFromMemberRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Revokes a role from a member of an organization";
            s.Description = "Revokes a role from a member of an organization with the given identifier";
            s.ExampleRequest = new RevokeRoleFromMemberRequest
            {
                OrganizationId = Guid.NewGuid(),
                MemberUserId = Guid.NewGuid(),
                RoleId = Guid.NewGuid()
            };
        });
    }

    /// <summary>
    /// Handles the DELETE /api/organizations/{organizationId}/members/{memberUserId}/roles/{roleId} request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Empty response in case of success.</returns>
    protected override async Task<EmptyResponse> HandleEndpointAsync(RevokeRoleFromMemberRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new RevokeRoleFromMemberCommand
        {
            OrganizationId = req.OrganizationId,
            MemberUserId = req.MemberUserId,
            RoleId = req.RoleId,
            RequestingUserId = userId
        };

        return await mediator.Send(command, ct);
    }
}
