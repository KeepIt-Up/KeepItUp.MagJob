using Ardalis.Specification;

namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;

/// <summary>
/// Specyfikacja do wyszukiwania roli wraz z jej członkami.
/// </summary>
public class RoleWithMembersSpec : Specification<Role>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="RoleWithMembersSpec"/>.
    /// </summary>
    /// <param name="roleId">ID roli do wyszukania.</param>
    public RoleWithMembersSpec(Guid roleId) =>
        Query
            .Where(role => role.Id == roleId)
            .Include(role => role.Members);
} 
