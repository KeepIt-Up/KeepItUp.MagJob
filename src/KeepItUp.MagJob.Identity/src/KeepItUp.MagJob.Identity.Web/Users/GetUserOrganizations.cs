using KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserOrganizations;
using KeepItUp.MagJob.Identity.Web.Services;
using Microsoft.AspNetCore.Authorization;

namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Endpoint do pobierania organizacji użytkownika.
/// </summary>
/// <remarks>
/// Pobiera wszystkie organizacje, do których należy użytkownik o podanym identyfikatorze.
/// </remarks>
[Authorize]
public class GetUserOrganizations(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<GetUserOrganizationsRequest, GetUserOrganizationsResponse>
{
    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Get(GetUserOrganizationsRequest.Route);
        Description(b => b
            .WithName("GetUserOrganizations")
            .Produces<GetUserOrganizationsResponse>(200)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s =>
        {
            s.Summary = "Pobiera organizacje użytkownika";
            s.Description = "Pobiera wszystkie organizacje, do których należy użytkownik o podanym identyfikatorze";
            s.ExampleRequest = new GetUserOrganizationsRequest { Id = Guid.NewGuid() };
        });
    }

    /// <summary>
    /// Obsługuje żądanie GET /api/users/{id}/organizations.
    /// </summary>
    /// <param name="req">Żądanie.</param>
    /// <param name="ct">Token anulowania.</param>
    /// <returns>Odpowiedź z listą organizacji użytkownika.</returns>
    public override async Task HandleAsync(GetUserOrganizationsRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetCurrentUserId();

        if (userId == null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var query = new GetUserOrganizationsQuery
        {
            UserId = req.Id
        };

        var result = await mediator.Send(query, ct);

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

        var response = new GetUserOrganizationsResponse
        {
            Organizations = result.Value.Select(o => new UserOrganizationRecord
            {
                Id = o.Id,
                Name = o.Name,
                Description = o.Description,
                OwnerId = o.OwnerId,
                IsOwner = o.OwnerId == req.Id,
                MemberCount = 0 // Tymczasowo ustawiam wartość domyślną
            }).ToList()
        };

        await SendOkAsync(response, ct);
    }
}
