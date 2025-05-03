namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

/// <summary>
/// Zdarzenie domenowe informujące o zaakceptowaniu zaproszenia do organizacji.
/// </summary>
public class InvitationAcceptedEvent : DomainEventBase
{
    /// <summary>
    /// Identyfikator zaproszenia.
    /// </summary>
    public Guid InvitationId { get; }

    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Adres e-mail osoby zapraszanej.
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Identyfikator roli, która zostanie przypisana.
    /// </summary>
    public Guid RoleId { get; }

    /// <summary>
    /// Tworzy nowe zdarzenie InvitationAcceptedEvent.
    /// </summary>
    /// <param name="invitationId">Identyfikator zaproszenia.</param>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="email">Adres e-mail osoby zapraszanej.</param>
    /// <param name="roleId">Identyfikator roli, która zostanie przypisana.</param>
    public InvitationAcceptedEvent(Guid invitationId, Guid organizationId, string email, Guid roleId)
    {
        InvitationId = invitationId;
        OrganizationId = organizationId;
        Email = email;
        RoleId = roleId;
    }
}
