
namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Odpowiedź z listą ról organizacji.
/// </summary>
public class GetOrganizationRolesResponse
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Lista ról organizacji.
    /// </summary>
    public List<KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.RoleDto> Roles { get; set; } = new();
}