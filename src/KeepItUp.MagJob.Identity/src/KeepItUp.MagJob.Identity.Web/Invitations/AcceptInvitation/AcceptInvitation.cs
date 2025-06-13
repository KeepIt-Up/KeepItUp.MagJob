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
    : Endpoint<AcceptInvitationRequest>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    public override void Configure()
    {
        Post(AcceptInvitationRequest.Route);
        AllowAnonymous();
        Description(b => b
            .WithName("AcceptInvitation")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(404)
            .ProducesProblem(500));
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
    public override async Task HandleAsync(AcceptInvitationRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new AcceptInvitationCommand
        {
            InvitationId = req.InvitationId,
            Token = req.Token,
            UserId = userId
        };

        var result = await mediator.Send(command, ct);

        if (result.Status == ResultStatus.NotFound)
        {
            await SendNotFoundAsync(ct);
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
