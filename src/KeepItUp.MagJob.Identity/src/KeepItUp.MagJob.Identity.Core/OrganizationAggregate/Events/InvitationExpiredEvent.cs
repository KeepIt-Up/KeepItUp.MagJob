namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

/// <summary>
/// Zdarzenie domenowe informujące o wygaśnięciu zaproszenia do organizacji.
/// </summary>
public class InvitationExpiredEvent : DomainEventBase
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
    /// Tworzy nowe zdarzenie InvitationExpiredEvent.
    /// </summary>
    /// <param name="invitationId">Identyfikator zaproszenia.</param>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="email">Adres e-mail osoby zapraszanej.</param>
    public InvitationExpiredEvent(Guid invitationId, Guid organizationId, string email)
    {
        InvitationId = invitationId;
        OrganizationId = organizationId;
        Email = email;
    }
}
