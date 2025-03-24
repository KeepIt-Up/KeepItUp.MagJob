
namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;

/// <summary>
/// Specyfikacja do pobierania organizacji, do których należy użytkownik.
/// </summary>
public class OrganizationsByUserIdSpec : Specification<Organization>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="OrganizationsByUserIdSpec"/>.
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika.</param>
    public OrganizationsByUserIdSpec(Guid userId)
    {
        Query
            .Where(o => o.Members.Any(m => m.UserId == userId))
            .Include(o => o.Members)
            .Include(o => o.Roles)
                .ThenInclude(r => r.Permissions);
    }
}
