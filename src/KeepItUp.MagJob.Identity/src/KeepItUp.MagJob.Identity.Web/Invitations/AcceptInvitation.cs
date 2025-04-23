using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.AcceptInvitation;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Invitations;

/// <summary>
/// Endpoint do akceptacji zaproszenia do organizacji.
/// </summary>
/// <remarks>
/// Akceptuje zaproszenie do organizacji na podstawie identyfikatora i tokenu.
/// </remarks>
public class AcceptInvitation(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<AcceptInvitationRequest>
{
    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Post(AcceptInvitationRequest.Route);
        AllowAnonymous(); // Tymczasowo, do czasu naprawienia autoryzacji
        Description(b => b
            .WithName("AcceptInvitation")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s =>
        {
            s.Summary = "Akceptuje zaproszenie do organizacji";
            s.Description = "Akceptuje zaproszenie do organizacji na podstawie identyfikatora i tokenu";
            s.ExampleRequest = new AcceptInvitationRequest { InvitationId = Guid.NewGuid(), Token = "token" };
        });
    }

    /// <summary>
    /// Obsługuje żądanie POST /api/invitations/{invitationId}/accept.
    /// </summary>
    /// <param name="req">Żądanie.</param>
    /// <param name="ct">Token anulowania.</param>
    /// <returns>Pusta odpowiedź w przypadku powodzenia.</returns>
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
