using Ardalis.Specification;

namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;

/// <summary>
/// Specyfikacja do wyszukiwania roli po nazwie i ID organizacji.
/// </summary>
public class RoleByNameAndOrgIdSpec : Specification<Role>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="RoleByNameAndOrgIdSpec"/>.
    /// </summary>
    /// <param name="name">Nazwa roli do wyszukania.</param>
    /// <param name="organizationId">ID organizacji do wyszukania.</param>
    public RoleByNameAndOrgIdSpec(string name, Guid organizationId) =>
        Query
            .Where(role => role.Name == name && role.OrganizationId == organizationId);
} 
