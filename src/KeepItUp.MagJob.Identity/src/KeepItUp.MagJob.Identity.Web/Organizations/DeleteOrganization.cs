using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.DeactivateOrganization;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint do usuwania (dezaktywacji) organizacji.
/// </summary>
/// <remarks>
/// Usuwa (dezaktywuje) organizację o podanym identyfikatorze.
/// </remarks>
public class DeleteOrganization(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<DeleteOrganizationRequest>
{
    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Delete(DeleteOrganizationRequest.Route);
        AllowAnonymous(); // Tymczasowo, do czasu naprawienia autoryzacji
        Description(b => b
            .WithName("DeleteOrganization")
            .Produces(204)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s =>
        {
            s.Summary = "Usuwa (dezaktywuje) organizację";
            s.Description = "Usuwa (dezaktywuje) organizację o podanym identyfikatorze";
        });
    }

    /// <summary>
    /// Obsługuje żądanie DELETE /api/organizations/{id}.
    /// </summary>
    /// <param name="req">Żądanie.</param>
    /// <param name="ct">Token anulowania.</param>
    /// <returns>Pusta odpowiedź w przypadku powodzenia.</returns>
    public override async Task HandleAsync(DeleteOrganizationRequest req, CancellationToken ct)
    {
        try
        {
            // Pobierz ID użytkownika z CurrentUserAccessor
            var userGuid = currentUserAccessor.GetRequiredCurrentUserId();

            var command = new DeactivateOrganizationCommand
            {
                Id = req.Id,
                UserId = userGuid
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

            await SendNoContentAsync(ct);
        }
        catch (UnauthorizedAccessException)
        {
            AddError("Nie można zidentyfikować użytkownika");
            await SendErrorsAsync(401, ct);
        }
    }
}
