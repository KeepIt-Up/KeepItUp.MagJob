
namespace KeepItUp.MagJob.Identity.Core.UserAggregate.Events;

/// <summary>
/// Zdarzenie informujące o dezaktywacji użytkownika.
/// </summary>
public class UserDeactivatedEvent : DomainEventBase
{
    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Identyfikator użytkownika w systemie zewnętrznym (Keycloak).
    /// </summary>
    public string ExternalId { get; }

    /// <summary>
    /// Adres e-mail użytkownika.
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Tworzy nowe zdarzenie informujące o dezaktywacji użytkownika.
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika.</param>
    /// <param name="externalId">Identyfikator użytkownika w systemie zewnętrznym (Keycloak).</param>
    /// <param name="email">Adres e-mail użytkownika.</param>
    public UserDeactivatedEvent(Guid userId, string externalId, string email)
    {
        UserId = userId;
        ExternalId = externalId;
        Email = email;
    }
}