using KeepItUp.MagJob.Identity.UseCases.Invitations.Commands.RejectInvitation;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Invitations;

/// <summary>
/// Endpoint to reject an invitation to an organization.
/// </summary>
/// <remarks>
/// Rejects an invitation to an organization based on an identifier and a token.
/// </remarks>
public class RejectInvitation(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : BaseEndpoint<RejectInvitationRequest, EmptyResponse>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    protected override void ConfigureEndpoint()
    {
        Post(RejectInvitationRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Rejects an invitation to an organization";
            s.Description = "Rejects an invitation to an organization based on an identifier and a token";
            s.ExampleRequest = new RejectInvitationRequest { InvitationId = Guid.NewGuid(), Token = "token" };
        });
    }

    /// <summary>
    /// Handles the POST /api/invitations/{invitationId}/reject request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Empty response in case of success.</returns>
    protected override async Task<EmptyResponse> HandleEndpointAsync(RejectInvitationRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new RejectInvitationCommand
        {
            InvitationId = req.InvitationId,
            Token = req.Token,
            UserId = userId
        };

        return await mediator.Send(command, ct);
    }
}
