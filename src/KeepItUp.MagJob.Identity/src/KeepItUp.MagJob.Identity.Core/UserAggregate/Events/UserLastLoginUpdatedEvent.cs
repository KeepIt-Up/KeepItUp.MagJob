namespace KeepItUp.MagJob.Identity.Core.UserAggregate.Events;

/// <summary>
/// Zdarzenie domenowe informujące o aktualizacji daty ostatniego logowania użytkownika.
/// </summary>
public class UserLastLoginUpdatedEvent : DomainEventBase
{
    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Identyfikator zewnętrzny użytkownika.
    /// </summary>
    public Guid ExternalId { get; }

    /// <summary>
    /// Adres e-mail użytkownika.
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Data ostatniego logowania użytkownika.
    /// </summary>
    public DateTime LastLoginDate { get; }

    /// <summary>
    /// Tworzy nowe zdarzenie UserLastLoginUpdatedEvent.
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika.</param>
    /// <param name="externalId">Identyfikator zewnętrzny użytkownika.</param>
    /// <param name="email">Adres e-mail użytkownika.</param>
    /// <param name="lastLoginDate">Data ostatniego logowania użytkownika.</param>
    public UserLastLoginUpdatedEvent(Guid userId, Guid externalId, string email, DateTime lastLoginDate)
    {
        UserId = userId;
        ExternalId = externalId;
        Email = email;
        LastLoginDate = lastLoginDate;
    }
}
