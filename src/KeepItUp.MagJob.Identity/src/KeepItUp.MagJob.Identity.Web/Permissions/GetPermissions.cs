using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetPermissions;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Permissions;

/// <summary>
/// Endpoint do pobierania wszystkich dostępnych uprawnień w systemie.
/// </summary>
/// <remarks>
/// Zwraca listę wszystkich dostępnych uprawnień w systemie.
/// </remarks>
public class GetPermissions(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<GetPermissionsRequest, PaginationResult<PermissionDto>>
{
    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Get(GetPermissionsRequest.Route);
        AllowAnonymous(); // Tymczasowo, do czasu naprawienia autoryzacji
        Description(b => b
            .WithName("GetPermissions")
            .Produces<PaginationResult<PermissionDto>>(200)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(500));
        Summary(s =>
        {
            s.Summary = "Pobiera wszystkie dostępne uprawnienia w systemie";
            s.Description = "Zwraca listę wszystkich dostępnych uprawnień w systemie";
            s.ExampleRequest = new GetPermissionsRequest
            {
                PaginationParameters = PaginationParameters<PermissionDto>.Create()
            };
        });
    }

    /// <summary>
    /// Obsługuje żądanie GET /api/permissions.
    /// </summary>
    /// <param name="req">Żądanie.</param>
    /// <param name="ct">Token anulowania.</param>
    /// <returns>Odpowiedź zawierająca listę uprawnień z paginacją.</returns>
    public override async Task HandleAsync(GetPermissionsRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var query = new GetPermissionsQuery
        {
            UserId = userId,
            PaginationParameters = req.PaginationParameters
        };

        var result = await mediator.Send(query, ct);

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

        await SendOkAsync(result.Value, ct);
    }
}
