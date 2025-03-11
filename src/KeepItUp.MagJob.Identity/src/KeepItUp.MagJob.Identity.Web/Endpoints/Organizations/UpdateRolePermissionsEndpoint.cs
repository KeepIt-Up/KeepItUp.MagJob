using Ardalis.Result;
using FastEndpoints;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateRolePermissions;
using KeepItUp.MagJob.Identity.Web.Services;
using MediatR;

namespace KeepItUp.MagJob.Identity.Web.Endpoints.Organizations;

/// <summary>
/// Endpoint do aktualizacji uprawnień roli w organizacji.
/// </summary>
/// <remarks>
/// Aktualizuje uprawnienia roli w organizacji o podanym identyfikatorze.
/// </remarks>
public class UpdateRolePermissionsEndpoint(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<UpdateRolePermissionsRequest, EmptyResponse>
{
    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Put("api/organizations/{organizationId}/roles/{roleId}/permissions");
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

/// <summary>
/// Żądanie aktualizacji uprawnień roli w organizacji.
/// </summary>
public class UpdateRolePermissionsRequest
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Identyfikator roli.
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Lista nazw uprawnień do przypisania do roli.
    /// </summary>
    public List<string> Permissions { get; set; } = new();
} 
