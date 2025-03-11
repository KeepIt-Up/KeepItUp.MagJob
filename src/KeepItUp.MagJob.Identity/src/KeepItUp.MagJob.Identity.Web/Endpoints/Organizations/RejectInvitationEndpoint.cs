using Ardalis.Result;
using FastEndpoints;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.RejectInvitation;
using KeepItUp.MagJob.Identity.Web.Services;
using MediatR;

namespace KeepItUp.MagJob.Identity.Web.Endpoints.Organizations;

/// <summary>
/// Endpoint do odrzucenia zaproszenia do organizacji.
/// </summary>
/// <remarks>
/// Odrzuca zaproszenie do organizacji na podstawie identyfikatora i tokenu.
/// </remarks>
public class RejectInvitationEndpoint(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<RejectInvitationRequest, EmptyResponse>
{
    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Post("api/invitations/{invitationId}/reject");
        AllowAnonymous(); // Tymczasowo, do czasu naprawienia autoryzacji
        Description(b => b
            .WithName("RejectInvitation")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s => {
            s.Summary = "Odrzuca zaproszenie do organizacji";
            s.Description = "Odrzuca zaproszenie do organizacji na podstawie identyfikatora i tokenu";
            s.ExampleRequest = new RejectInvitationRequest { InvitationId = Guid.NewGuid(), Token = "token" };
        });
    }

    /// <summary>
    /// Obsługuje żądanie POST /api/invitations/{invitationId}/reject.
    /// </summary>
    /// <param name="req">Żądanie.</param>
    /// <param name="ct">Token anulowania.</param>
    /// <returns>Pusta odpowiedź w przypadku powodzenia.</returns>
    public override async Task HandleAsync(RejectInvitationRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new RejectInvitationCommand
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

/// <summary>
/// Żądanie odrzucenia zaproszenia do organizacji.
/// </summary>
public class RejectInvitationRequest
{
    /// <summary>
    /// Identyfikator zaproszenia.
    /// </summary>
    public Guid InvitationId { get; set; }

    /// <summary>
    /// Token zaproszenia.
    /// </summary>
    public string Token { get; set; } = string.Empty;
} 
