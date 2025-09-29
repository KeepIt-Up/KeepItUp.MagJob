namespace KeepItUp.MagJob.Identity.UseCases.Permissions.Queries.GetPermissions;

/// <summary>
/// Query to get all available permissions in the system.
/// </summary>
public class GetPermissionsQuery : PaginationQuery<PermissionDto>
{
    /// <summary>
    /// User identifier performing the query.
    /// </summary>
    public Guid UserId { get; init; }
}
