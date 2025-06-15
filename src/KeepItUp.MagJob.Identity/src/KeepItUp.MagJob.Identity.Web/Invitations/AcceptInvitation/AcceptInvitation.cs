using KeepItUp.MagJob.Identity.UseCases.Invitations.Commands.AcceptInvitation;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Invitations;

/// <summary>
/// Endpoint to accept an invitation to an organization.
/// </summary>
/// <remarks>
/// Accepts an invitation to an organization based on the identifier and token.
/// </remarks>
public class AcceptInvitation(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : BaseEndpoint<AcceptInvitationRequest, EmptyResponse>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    protected override void ConfigureEndpoint()
    {
        Post(AcceptInvitationRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Accepts an invitation to an organization";
            s.Description = "Accepts an invitation to an organization based on the identifier and token";
            s.ExampleRequest = new AcceptInvitationRequest { InvitationId = Guid.NewGuid(), Token = "token" };
        });
    }

    /// <summary>
    /// Handles the POST /api/invitations/{invitationId}/accept request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Empty response in case of success.</returns>
    protected override async Task<EmptyResponse> HandleEndpointAsync(AcceptInvitationRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new AcceptInvitationCommand
        {
            InvitationId = req.InvitationId,
            Token = req.Token,
            UserId = userId
        };

        return await mediator.Send(command, ct);
    }
}
