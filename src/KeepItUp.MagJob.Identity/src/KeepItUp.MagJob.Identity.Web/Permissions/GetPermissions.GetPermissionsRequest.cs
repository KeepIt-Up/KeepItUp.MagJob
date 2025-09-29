using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetPermissions;

namespace KeepItUp.MagJob.Identity.Web.Permissions;

/// <summary>
/// Request for the GetPermissionsEndpoint.
/// </summary>
public class GetPermissionsRequest : PaginationRequest<PermissionDto>
{
    public const string Route = "/Permissions";
}
