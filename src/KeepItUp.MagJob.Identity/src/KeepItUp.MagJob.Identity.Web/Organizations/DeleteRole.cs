using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.DeleteRole;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint do usunięcia roli z organizacji.
/// </summary>
/// <remarks>
/// Usuwa rolę z organizacji o podanym identyfikatorze.
/// </remarks>
public class DeleteRole(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<DeleteRoleRequest>
{
    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Delete(DeleteRoleRequest.Route);
        AllowAnonymous(); // Tymczasowo, do czasu naprawienia autoryzacji
        Description(b => b
            .WithName("DeleteRole")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s => {
            s.Summary = "Usuwa rolę z organizacji";
            s.Description = "Usuwa rolę z organizacji o podanym identyfikatorze";
            s.ExampleRequest = new DeleteRoleRequest { OrganizationId = Guid.NewGuid(), RoleId = Guid.NewGuid() };
        });
    }

    /// <summary>
    /// Obsługuje żądanie DELETE /api/organizations/{organizationId}/roles/{roleId}.
    /// </summary>
    /// <param name="req">Żądanie.</param>
    /// <param name="ct">Token anulowania.</param>
    /// <returns>Pusta odpowiedź w przypadku powodzenia.</returns>
    public override async Task HandleAsync(DeleteRoleRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new DeleteRoleCommand
        {
            OrganizationId = req.OrganizationId,
            RoleId = req.RoleId,
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
