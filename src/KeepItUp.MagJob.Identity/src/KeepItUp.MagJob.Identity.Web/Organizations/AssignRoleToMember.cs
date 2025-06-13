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
    : Endpoint<AssignRoleToMemberRequest>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    public override void Configure()
    {
        Post(AssignRoleToMemberRequest.Route);
        AllowAnonymous();
        Description(b => b
            .WithName("AssignRoleToMember")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
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
    public override async Task HandleAsync(AssignRoleToMemberRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new AssignRoleToMemberCommand
        {
            OrganizationId = req.OrganizationId,
            MemberUserId = req.MemberUserId,
            RoleId = req.RoleId,
            RequestingUserId = userId
        };

        var result = await mediator.Send(command, ct);

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

        if (result.Status == ResultStatus.Invalid)
        {
            foreach (var error in result.ValidationErrors)
            {
                AddError(error.ErrorMessage);
            }
            await SendErrorsAsync(400, ct);
            return;
        }

        await SendNoContentAsync(ct);
    }
}
