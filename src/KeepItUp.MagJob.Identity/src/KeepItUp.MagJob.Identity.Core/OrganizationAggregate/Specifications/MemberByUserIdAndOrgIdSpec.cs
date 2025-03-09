using Ardalis.Specification;

namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;

/// <summary>
/// Specyfikacja do wyszukiwania członka organizacji po ID użytkownika i ID organizacji.
/// </summary>
public class MemberByUserIdAndOrgIdSpec : Specification<Member>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="MemberByUserIdAndOrgIdSpec"/>.
    /// </summary>
    /// <param name="userId">ID użytkownika do wyszukania.</param>
    /// <param name="organizationId">ID organizacji do wyszukania.</param>
    public MemberByUserIdAndOrgIdSpec(Guid userId, Guid organizationId) =>
        Query
            .Where(member => member.UserId == userId && member.OrganizationId == organizationId);
} 
