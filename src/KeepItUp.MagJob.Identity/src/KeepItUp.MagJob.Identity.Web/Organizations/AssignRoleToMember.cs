using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.AssignRoleToMember;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint to assign a role to a member of an organization.
/// </summary>
/// <remarks>
/// Assigns a role to a member of an organization with the given identifier.
/// </remarks>
public class AssignRoleToMember(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : BaseEndpoint<AssignRoleToMemberRequest, EmptyResponse>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    protected override void ConfigureEndpoint()
    {
        Post(AssignRoleToMemberRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Assigns a role to a member of an organization";
            s.Description = "Assigns a role to a member of an organization with the given identifier";
            s.ExampleRequest = new AssignRoleToMemberRequest
            {
                OrganizationId = Guid.NewGuid(),
                MemberUserId = Guid.NewGuid(),
                RoleId = Guid.NewGuid()
            };
        });
    }

    /// <summary>
    /// Handles the POST /api/organizations/{organizationId}/members/{memberUserId}/roles request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Empty response in case of success.</returns>
    protected override async Task<EmptyResponse> HandleEndpointAsync(AssignRoleToMemberRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new AssignRoleToMemberCommand
        {
            OrganizationId = req.OrganizationId,
            MemberUserId = req.MemberUserId,
            RoleId = req.RoleId,
            RequestingUserId = userId
        };

        return await mediator.Send(command, ct);
    }
}
