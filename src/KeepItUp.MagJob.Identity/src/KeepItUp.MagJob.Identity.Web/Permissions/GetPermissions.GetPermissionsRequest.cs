using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetPermissions;

namespace KeepItUp.MagJob.Identity.Web.Permissions;

/// <summary>
/// Żądanie dla endpointu GetPermissions.
/// </summary>
public class GetPermissionsRequest : PaginationRequest<PermissionDto>
{
    public const string Route = "/Permissions";
}
