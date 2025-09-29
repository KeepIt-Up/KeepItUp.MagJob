namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetPermissions;

/// <summary>
/// DTO for a permission.
/// </summary>
public class PermissionDto
{
    /// <summary>
    /// Permission name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Permission description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Permission category.
    /// </summary>
    public string Category { get; set; } = string.Empty;
}

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
