using Ardalis.Specification;

namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;

/// <summary>
/// Specyfikacja do wyszukiwania członka organizacji po ID użytkownika.
/// </summary>
public class MemberByUserIdSpec : Specification<Member>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="MemberByUserIdSpec"/>.
    /// </summary>
    /// <param name="userId">ID użytkownika do wyszukania.</param>
    public MemberByUserIdSpec(Guid userId) =>
        Query
            .Where(member => member.UserId == userId);
} 
