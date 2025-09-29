using KeepItUp.MagJob.Identity.UseCases.Invitations.Commands.CreateInvitation;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Invitations;

/// <summary>
/// Endpoint to create an invitation to an organization.
/// </summary>
/// <remarks>
/// Creates a new invitation to an organization for the given email address.
/// </remarks>
public class CreateInvitation(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
  : BaseEndpoint<CreateInvitationRequest, Guid>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    protected override void ConfigureEndpoint()
    {
        Post(CreateInvitationRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Create an invitation to an organization";
            s.Description = "Create an invitation to an organization for the given email address";
            s.ExampleRequest = new CreateInvitationRequest { OrganizationId = Guid.NewGuid(), Email = "example@example.com", RoleId = Guid.NewGuid() };
            s.ResponseExamples[201] = Guid.NewGuid();
        });
    }

    /// <summary>
    /// Handles the POST /api/organizations/{organizationId}/invitations request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Response containing the identifier of the created invitation.</returns>
    protected override async Task<Guid> HandleEndpointAsync(CreateInvitationRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new CreateInvitationCommand()
        {
            OrganizationId = req.OrganizationId,
            Email = req.Email,
            RoleId = req.RoleId,
            UserId = userId
        };

        return await mediator.Send(command, ct);
    }
}
