
namespace KeepItUp.MagJob.Identity.UseCases.Permissions.Queries;

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