namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationInvitations;

/// <summary>
/// Zapytanie o zaproszenia do organizacji.
/// </summary>
public class GetOrganizationInvitationsQuery : PaginationQuery<InvitationDto>
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Identyfikator użytkownika wykonującego zapytanie.
    /// </summary>
    public Guid UserId { get; init; }
}
