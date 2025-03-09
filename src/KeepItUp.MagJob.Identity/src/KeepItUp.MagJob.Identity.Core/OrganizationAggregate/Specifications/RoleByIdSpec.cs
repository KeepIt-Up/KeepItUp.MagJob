using Ardalis.Specification;

namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;

/// <summary>
/// Specyfikacja do wyszukiwania roli po ID.
/// </summary>
public class RoleByIdSpec : Specification<Role>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="RoleByIdSpec"/>.
    /// </summary>
    /// <param name="roleId">ID roli do wyszukania.</param>
    public RoleByIdSpec(Guid roleId) =>
        Query
            .Where(role => role.Id == roleId);
} 
