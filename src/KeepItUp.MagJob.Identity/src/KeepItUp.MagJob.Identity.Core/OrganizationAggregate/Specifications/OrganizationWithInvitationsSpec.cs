
namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;

/// <summary>
/// Specyfikacja do pobierania organizacji wraz z jej zaproszeniami.
/// </summary>
public class OrganizationWithInvitationsSpec : Specification<Organization>, ISingleResultSpecification<Organization>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="OrganizationWithInvitationsSpec"/>.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    public OrganizationWithInvitationsSpec(Guid organizationId)
    {
        Query
            .Where(o => o.Id == organizationId)
            .Include(o => o.Invitations)
            .Include(o => o.Roles)
            .Include(o => o.Members);
    }
} 
