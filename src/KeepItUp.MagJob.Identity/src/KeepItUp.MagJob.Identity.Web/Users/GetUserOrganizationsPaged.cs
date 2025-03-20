
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;

using KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserOrganizationsPaged;
using KeepItUp.MagJob.Identity.Web.Services;
using KeepItUp.MagJob.SharedKernel.Pagination;
using Microsoft.AspNetCore.Authorization;

namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Endpoint do pobierania stronicowanej listy organizacji użytkownika.
/// </summary>
/// <remarks>
/// Pobiera stronicowaną listę organizacji, do których należy użytkownik o podanym identyfikatorze.
/// </remarks>
[Authorize]
public class GetUserOrganizationsPaged(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<GetUserOrganizationsPagedRequest, PaginationResult<OrganizationDto>>
{
    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Get(GetUserOrganizationsPagedRequest.Route);
        Description(b => b
            .WithName("GetUserOrganizationsPaged")
            .Produces<PaginationResult<OrganizationDto>>(200)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s =>
        {
            s.Summary = "Pobiera stronicowaną listę organizacji użytkownika";
            s.Description = "Pobiera stronicowaną listę organizacji, do których należy użytkownik o podanym identyfikatorze";
            s.ExampleRequest = new GetUserOrganizationsPagedRequest
            {
                Id = Guid.NewGuid(),
                PaginationParameters = PaginationParameters<OrganizationDto>.Create()
            };
        });
    }

    /// <summary>
    /// Obsługuje żądanie GET /api/users/{id}/organizations/paged.
    /// </summary>
    /// <param name="req">Żądanie.</param>
    /// <param name="ct">Token anulowania.</param>
    /// <returns>Odpowiedź z stronicowaną listą organizacji użytkownika.</returns>
    public override async Task HandleAsync(GetUserOrganizationsPagedRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetCurrentUserId();

        if (userId == null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var query = new GetUserOrganizationsPagedQuery
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

        var response = result.Value;


        await SendOkAsync(response, ct);
    }
}
