
namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;

/// <summary>
/// Specyfikacja pobierająca organizację wraz z jej członkami i rolami.
/// </summary>
public class OrganizationWithMembersAndRolesSpec : Specification<Organization>, ISingleResultSpecification<Organization>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="OrganizationWithMembersAndRolesSpec"/>.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    public OrganizationWithMembersAndRolesSpec(Guid organizationId)
    {
        Query
            .Where(o => o.Id == organizationId)
            .Include(o => o.Members)
            .Include(o => o.Roles)
            .ThenInclude(r => r.Permissions);
    }
} 
