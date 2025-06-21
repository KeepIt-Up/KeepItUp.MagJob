using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
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
    : Endpoint<GetUserOrganizationsRequest, PaginationResult<OrganizationDto>>
{
    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Get(GetUserOrganizationsRequest.Route);
        Description(b => b
            .WithName("GetUserOrganizations")
            .Produces<PaginationResult<OrganizationDto>>(200));
        Summary(s =>
        {
            s.Summary = "Pobiera organizacje użytkownika";
            s.Description = "Pobiera wszystkie organizacje, do których należy użytkownik o podanym identyfikatorze";
            s.ExampleRequest = new GetUserOrganizationsRequest
            {
                Id = Guid.NewGuid(),
                PaginationParameters = PaginationParameters<OrganizationDto>.Create()
            };
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
            UserId = req.Id,
            PaginationParameters = req.PaginationParameters
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

        await SendOkAsync(result.Value, ct);
    }
}
