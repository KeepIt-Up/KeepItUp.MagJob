using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateRolePermissions;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint do aktualizacji uprawnień roli w organizacji.
/// </summary>
/// <remarks>
/// Aktualizuje uprawnienia roli w organizacji o podanym identyfikatorze.
/// </remarks>
public class UpdateRolePermissions(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<UpdateRolePermissionsRequest>
{
    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Put(UpdateRolePermissionsRequest.Route);
        AllowAnonymous(); // Tymczasowo, do czasu naprawienia autoryzacji
        Description(b => b
            .WithName("UpdateRolePermissions")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s => {
            s.Summary = "Aktualizuje uprawnienia roli w organizacji";
            s.Description = "Aktualizuje uprawnienia roli w organizacji o podanym identyfikatorze";
            s.ExampleRequest = new UpdateRolePermissionsRequest { 
                OrganizationId = Guid.NewGuid(), 
                RoleId = Guid.NewGuid(),
                Permissions = new List<string> { "organization.create", "organization.update" }
            };
        });
    }

    /// <summary>
    /// Obsługuje żądanie PUT /api/organizations/{organizationId}/roles/{roleId}/permissions.
    /// </summary>
    /// <param name="req">Żądanie.</param>
    /// <param name="ct">Token anulowania.</param>
    /// <returns>Pusta odpowiedź w przypadku powodzenia.</returns>
    public override async Task HandleAsync(UpdateRolePermissionsRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new UpdateRolePermissionsCommand
        {
            OrganizationId = req.OrganizationId,
            RoleId = req.RoleId,
            Permissions = req.Permissions,
            UserId = userId
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
