using Ardalis.Result;
using FastEndpoints;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.CreateRole;
using KeepItUp.MagJob.Identity.Web.Services;
using MediatR;

namespace KeepItUp.MagJob.Identity.Web.Endpoints.Organizations;

/// <summary>
/// Endpoint do tworzenia nowej roli w organizacji.
/// </summary>
/// <remarks>
/// Tworzy nową rolę w organizacji o podanym identyfikatorze.
/// </remarks>
public class CreateRoleEndpoint(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<CreateRoleRequest, CreateRoleResponse>
{
    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Post("api/organizations/{organizationId}/roles");
        AllowAnonymous(); // Tymczasowo, do czasu naprawienia autoryzacji
        Description(b => b
            .WithName("CreateRole")
            .Produces(201)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s => {
            s.Summary = "Tworzy nową rolę w organizacji";
            s.Description = "Tworzy nową rolę w organizacji o podanym identyfikatorze";
            s.ExampleRequest = new CreateRoleRequest { 
                OrganizationId = Guid.NewGuid(), 
                Name = "Administrator", 
                Description = "Rola administratora organizacji", 
                Color = "#FF0000" 
            };
            s.ResponseExamples[201] = new CreateRoleResponse { 
                Id = Guid.NewGuid(), 
                Name = "Administrator" 
            };
        });
    }

    /// <summary>
    /// Obsługuje żądanie POST /api/organizations/{organizationId}/roles.
    /// </summary>
    /// <param name="req">Żądanie.</param>
    /// <param name="ct">Token anulowania.</param>
    /// <returns>Odpowiedź zawierająca identyfikator utworzonej roli.</returns>
    public override async Task HandleAsync(CreateRoleRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new CreateRoleCommand
        {
            OrganizationId = req.OrganizationId,
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

        Response = new CreateRoleResponse
        {
            Id = result.Value,
            Name = req.Name
        };

        await SendCreatedAtAsync<GetOrganizationRolesEndpoint>(
            new { organizationId = req.OrganizationId },
            Response,
            generateAbsoluteUrl: true,
            cancellation: ct);
    }
}

/// <summary>
/// Żądanie utworzenia nowej roli w organizacji.
/// </summary>
public class CreateRoleRequest
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; set; }

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

/// <summary>
/// Odpowiedź zawierająca identyfikator utworzonej roli.
/// </summary>
public class CreateRoleResponse
{
    /// <summary>
    /// Identyfikator utworzonej roli.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nazwa roli.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
