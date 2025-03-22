using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganization;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Endpoints.Organizations;

/// <summary>
/// Endpoint do aktualizacji organizacji.
/// </summary>
/// <remarks>
/// Aktualizuje istniejącą organizację o podanym identyfikatorze.
/// </remarks>
public class UpdateOrganization(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<UpdateOrganizationRequest, UpdateOrganizationResponse>
{
    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Put(UpdateOrganizationRequest.Route);
        AllowAnonymous(); // Tymczasowo, do czasu naprawienia autoryzacji
        Description(b => b
            .WithName("UpdateOrganization")
            .Produces<UpdateOrganizationResponse>(200)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s => {
            s.Summary = "Aktualizuje istniejącą organizację";
            s.Description = "Aktualizuje istniejącą organizację o podanym identyfikatorze";
            s.ExampleRequest = new UpdateOrganizationRequest { Id = Guid.NewGuid(), Name = "Nowa nazwa organizacji", Description = "Nowy opis organizacji" };
            s.ResponseExamples[200] = new UpdateOrganizationResponse { Id = Guid.NewGuid(), Name = "Nowa nazwa organizacji", Description = "Nowy opis organizacji", OwnerId = Guid.NewGuid() };
        });
    }

    /// <summary>
    /// Obsługuje żądanie PUT /api/organizations/{id}.
    /// </summary>
    /// <param name="req">Żądanie.</param>
    /// <param name="ct">Token anulowania.</param>
    /// <returns>Odpowiedź z danymi zaktualizowanej organizacji.</returns>
    public override async Task HandleAsync(UpdateOrganizationRequest req, CancellationToken ct)
    {
        try
        {
            // Pobierz ID użytkownika z CurrentUserAccessor
            var userGuid = currentUserAccessor.GetRequiredCurrentUserId();

            var command = new UpdateOrganizationCommand
            {
                Id = req.Id,
                Name = req.Name,
                Description = req.Description,
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

            if (result.Status == ResultStatus.Invalid)
            {
                foreach (var error in result.ValidationErrors)
                {
                    AddError(error.ErrorMessage);
                }
                await SendErrorsAsync(400, ct);
                return;
            }

            Response = new UpdateOrganizationResponse
            {
                Id = req.Id,
                Name = req.Name,
                Description = req.Description,
                OwnerId = userGuid // Zakładamy, że użytkownik aktualizujący jest właścicielem
            };

            await SendOkAsync(Response, ct);
        }
        catch (UnauthorizedAccessException)
        {
            AddError("Nie można zidentyfikować użytkownika");
            await SendErrorsAsync(401, ct);
        }
    }
} 
