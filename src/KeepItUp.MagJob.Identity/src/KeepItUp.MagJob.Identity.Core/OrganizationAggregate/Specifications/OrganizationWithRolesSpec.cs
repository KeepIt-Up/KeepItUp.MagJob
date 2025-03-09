using Ardalis.Specification;

namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;

/// <summary>
/// Specyfikacja do wyszukiwania organizacji wraz z jej rolami.
/// </summary>
public class OrganizationWithRolesSpec : Specification<Organization>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="OrganizationWithRolesSpec"/>.
    /// </summary>
    /// <param name="organizationId">ID organizacji do wyszukania.</param>
    public OrganizationWithRolesSpec(Guid organizationId) =>
        Query
            .Where(org => org.Id == organizationId)
            .Include(org => org.Roles);
} 
