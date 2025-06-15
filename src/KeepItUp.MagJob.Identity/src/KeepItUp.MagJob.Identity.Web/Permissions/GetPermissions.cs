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
    : BaseEndpoint<GetPermissionsRequest, PaginationResult<PermissionDto>>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    protected override void ConfigureEndpoint()
    {
        Get(GetPermissionsRequest.Route);
        AllowAnonymous();
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
    protected override async Task<PaginationResult<PermissionDto>> HandleEndpointAsync(GetPermissionsRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var query = new GetPermissionsQuery
        {
            UserId = userId,
            PaginationParameters = req.PaginationParameters
        };

        return await mediator.Send(query, ct);
    }
}
