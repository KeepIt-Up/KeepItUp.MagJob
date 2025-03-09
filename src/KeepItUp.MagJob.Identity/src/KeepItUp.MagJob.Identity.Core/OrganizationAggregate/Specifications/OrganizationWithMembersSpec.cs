using Ardalis.Specification;

namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;

/// <summary>
/// Specyfikacja do wyszukiwania organizacji wraz z jej członkami.
/// </summary>
public class OrganizationWithMembersSpec : Specification<Organization>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="OrganizationWithMembersSpec"/>.
    /// </summary>
    /// <param name="organizationId">ID organizacji do wyszukania.</param>
    public OrganizationWithMembersSpec(Guid organizationId) =>
        Query
            .Where(org => org.Id == organizationId)
            .Include(org => org.Members);
} 
