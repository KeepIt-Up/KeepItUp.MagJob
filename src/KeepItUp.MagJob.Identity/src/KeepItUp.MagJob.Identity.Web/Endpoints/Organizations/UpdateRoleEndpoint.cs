using Ardalis.Result;
using FastEndpoints;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateRole;
using KeepItUp.MagJob.Identity.Web.Services;
using MediatR;

namespace KeepItUp.MagJob.Identity.Web.Endpoints.Organizations;

/// <summary>
/// Endpoint do aktualizacji roli w organizacji.
/// </summary>
/// <remarks>
/// Aktualizuje rolę w organizacji o podanym identyfikatorze.
/// </remarks>
public class UpdateRoleEndpoint(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<UpdateRoleRequest, EmptyResponse>
{
    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Put("api/organizations/{organizationId}/roles/{roleId}");
        AllowAnonymous(); // Tymczasowo, do czasu naprawienia autoryzacji
        Description(b => b
            .WithName("UpdateRole")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s => {
            s.Summary = "Aktualizuje rolę w organizacji";
            s.Description = "Aktualizuje rolę w organizacji o podanym identyfikatorze";
            s.ExampleRequest = new UpdateRoleRequest { 
                OrganizationId = Guid.NewGuid(), 
                RoleId = Guid.NewGuid(),
                Name = "Administrator", 
                Description = "Rola administratora organizacji", 
                Color = "#FF0000" 
            };
        });
    }

    /// <summary>
    /// Obsługuje żądanie PUT /api/organizations/{organizationId}/roles/{roleId}.
    /// </summary>
    /// <param name="req">Żądanie.</param>
    /// <param name="ct">Token anulowania.</param>
    /// <returns>Pusta odpowiedź w przypadku powodzenia.</returns>
    public override async Task HandleAsync(UpdateRoleRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new UpdateRoleCommand
        {
            OrganizationId = req.OrganizationId,
            RoleId = req.RoleId,
            Name = req.Name,
            Description = req.Description,
            Color = req.Color,
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
/// Żądanie aktualizacji roli w organizacji.
/// </summary>
public class UpdateRoleRequest
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
    /// Nazwa roli.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Opis roli.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Kolor roli (w formacie HEX).
    /// </summary>
    public string? Color { get; set; }
}

