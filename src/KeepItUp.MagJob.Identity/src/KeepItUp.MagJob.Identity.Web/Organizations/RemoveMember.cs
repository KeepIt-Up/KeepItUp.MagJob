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
    : Endpoint<RemoveMemberRequest>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    public override void Configure()
    {
        Delete(RemoveMemberRequest.Route);
        AllowAnonymous();
        Description(b => b
            .WithName("RemoveMember")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
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
    public override async Task HandleAsync(RemoveMemberRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new RemoveMemberCommand
        {
            OrganizationId = req.OrganizationId,
            MemberUserId = req.MemberUserId,
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
