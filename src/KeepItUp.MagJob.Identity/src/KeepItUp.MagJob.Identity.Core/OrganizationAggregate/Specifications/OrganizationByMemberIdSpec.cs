using Ardalis.Specification;

namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;

/// <summary>
/// Specyfikacja do wyszukiwania organizacji na podstawie ID członka.
/// </summary>
public class OrganizationByMemberIdSpec : Specification<Organization>
{
    public OrganizationByMemberIdSpec(Guid memberId)
    {
        Query.Where(org => org.Members.Any(m => m.Id == memberId));
    }
}