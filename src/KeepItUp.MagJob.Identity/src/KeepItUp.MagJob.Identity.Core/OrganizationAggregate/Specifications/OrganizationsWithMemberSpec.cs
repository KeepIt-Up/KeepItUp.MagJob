
namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;

/// <summary>
/// Specyfikacja do pobierania organizacji, w których użytkownik jest członkiem.
/// </summary>
public class OrganizationsWithMemberSpec : Specification<Organization>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="OrganizationsWithMemberSpec"/>.
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika.</param>
    public OrganizationsWithMemberSpec(Guid userId)
    {
        Query
            .Include(o => o.Members)
                .ThenInclude(m => m.Roles)
            .Where(o => o.Members.Any(m => m.UserId == userId))
            .Where(o => o.IsActive);
    }
} 
