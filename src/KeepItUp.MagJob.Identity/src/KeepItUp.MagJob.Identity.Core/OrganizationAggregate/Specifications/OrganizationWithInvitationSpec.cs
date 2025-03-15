
namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;

/// <summary>
/// Specyfikacja do pobierania organizacji wraz z konkretnym zaproszeniem.
/// </summary>
public class OrganizationWithInvitationSpec : Specification<Organization>, ISingleResultSpecification<Organization>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="OrganizationWithInvitationSpec"/>.
    /// </summary>
    /// <param name="invitationId">Identyfikator zaproszenia.</param>
    public OrganizationWithInvitationSpec(Guid invitationId)
    {
        Query
            .Include(o => o.Invitations)
            .Include(o => o.Members)
            .Include(o => o.Roles)
            .Where(o => o.Invitations.Any(i => i.Id == invitationId));
    }
} 
