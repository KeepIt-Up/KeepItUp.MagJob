
namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Response containing the list of roles of the organization.
/// </summary>
public class GetOrganizationRolesResponse
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// List of roles of the organization.
    /// </summary>
    public List<KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.RoleDto> Roles { get; set; } = new();
}