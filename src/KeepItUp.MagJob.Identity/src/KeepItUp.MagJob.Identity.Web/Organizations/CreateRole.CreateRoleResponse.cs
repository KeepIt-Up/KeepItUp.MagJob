
namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Response containing the identifier of the created role.
/// </summary>
public class CreateRoleResponse
{
    /// <summary>
    /// Identifier of the created role.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Role name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}