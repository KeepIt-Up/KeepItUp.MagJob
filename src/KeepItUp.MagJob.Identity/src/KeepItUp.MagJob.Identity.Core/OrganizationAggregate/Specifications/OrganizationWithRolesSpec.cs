
namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;

/// <summary>
/// Specyfikacja do pobierania organizacji wraz z jej rolami.
/// </summary>
public class OrganizationWithRolesSpec : Specification<Organization>, ISingleResultSpecification<Organization>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="OrganizationWithRolesSpec"/>.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    public OrganizationWithRolesSpec(Guid organizationId)
    {
        Query
            .Where(o => o.Id == organizationId)
            .Include(o => o.Roles)
            .Include(o => o.Members)
                .ThenInclude(m => m.Roles)
            .Include(o => o.Invitations);
    }
} 
