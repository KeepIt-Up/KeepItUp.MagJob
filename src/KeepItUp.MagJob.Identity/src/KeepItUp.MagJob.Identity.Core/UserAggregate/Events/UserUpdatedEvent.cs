
namespace KeepItUp.MagJob.Identity.Core.UserAggregate.Events;

/// <summary>
/// Zdarzenie informujące o aktualizacji danych użytkownika.
/// </summary>
public class UserUpdatedEvent : DomainEventBase
{
    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Identyfikator użytkownika w systemie zewnętrznym (Keycloak).
    /// </summary>
    public Guid ExternalId { get; }

    /// <summary>
    /// Adres e-mail użytkownika.
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Tworzy nowe zdarzenie informujące o aktualizacji danych użytkownika.
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika.</param>
    /// <param name="externalId">Identyfikator użytkownika w systemie zewnętrznym (Keycloak).</param>
    /// <param name="email">Adres e-mail użytkownika.</param>
    public UserUpdatedEvent(Guid userId, Guid externalId, string email)
    {
        UserId = userId;
        ExternalId = externalId;
        Email = email;
    }
}
