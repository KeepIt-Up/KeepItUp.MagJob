using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetPermissions;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Permissions;

/// <summary>
/// Endpoint to get all available permissions in the system.
/// </summary>
/// <remarks>
/// Returns a list of all available permissions in the system.
/// </remarks>
public class GetPermissions(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<GetPermissionsRequest, PaginationResult<PermissionDto>>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    public override void Configure()
    {
        Get(GetPermissionsRequest.Route);
        AllowAnonymous();
        Description(b => b
            .WithName("GetPermissions")
            .Produces<PaginationResult<PermissionDto>>(200)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(500));
        Summary(s =>
        {
            s.Summary = "Gets all available permissions in the system";
            s.Description = "Returns a list of all available permissions in the system";
            s.ExampleRequest = new GetPermissionsRequest
            {
                PaginationParameters = PaginationParameters<PermissionDto>.Create()
            };
        });
    }

    /// <summary>
    /// Handles the GET /api/permissions request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Response containing a list of permissions with pagination.</returns>
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
