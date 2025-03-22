using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.RemoveMember;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint do usunięcia członka z organizacji.
/// </summary>
/// <remarks>
/// Usuwa członka z organizacji o podanym identyfikatorze.
/// </remarks>
public class RemoveMember(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<RemoveMemberRequest>
{ 
    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Delete(RemoveMemberRequest.Route);
        AllowAnonymous(); // Tymczasowo, do czasu naprawienia autoryzacji
        Description(b => b
            .WithName("RemoveMember")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s => {
            s.Summary = "Usuwa członka z organizacji";
            s.Description = "Usuwa członka z organizacji o podanym identyfikatorze";
            s.ExampleRequest = new RemoveMemberRequest { OrganizationId = Guid.NewGuid(), MemberUserId = Guid.NewGuid() };
        });
    }

    /// <summary>
    /// Obsługuje żądanie DELETE /api/organizations/{organizationId}/members/{memberUserId}.
    /// </summary>
    /// <param name="req">Żądanie.</param>
    /// <param name="ct">Token anulowania.</param>
    /// <returns>Pusta odpowiedź w przypadku powodzenia.</returns>
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
