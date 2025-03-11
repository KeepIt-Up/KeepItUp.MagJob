using Ardalis.Specification;

namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;

/// <summary>
/// Specyfikacja do pobierania organizacji wraz z jej członkami.
/// </summary>
public class OrganizationWithMembersSpec : Specification<Organization>, ISingleResultSpecification<Organization>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="OrganizationWithMembersSpec"/>.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    public OrganizationWithMembersSpec(Guid organizationId)
    {
        Query
            .Where(o => o.Id == organizationId)
            .Include(o => o.Members)
                .ThenInclude(m => m.Roles);
    }
} 
