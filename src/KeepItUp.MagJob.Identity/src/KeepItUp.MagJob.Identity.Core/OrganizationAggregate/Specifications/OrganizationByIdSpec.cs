using Ardalis.Specification;

namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;

/// <summary>
/// Specyfikacja do wyszukiwania organizacji po ID.
/// </summary>
public class OrganizationByIdSpec : Specification<Organization>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="OrganizationByIdSpec"/>.
    /// </summary>
    /// <param name="organizationId">ID organizacji do wyszukania.</param>
    public OrganizationByIdSpec(Guid organizationId) =>
        Query
            .Where(org => org.Id == organizationId);
} 
