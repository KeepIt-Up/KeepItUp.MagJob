
namespace KeepItUp.MagJob.Identity.Core.UserAggregate.Events;

/// <summary>
/// Zdarzenie emitowane, gdy uprawnienia użytkownika zostają zaktualizowane.
/// </summary>
public class UserPermissionsUpdatedEvent : DomainEventBase
{
    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Identyfikator zewnętrzny użytkownika.
    /// </summary>
    public string ExternalId { get; }

    /// <summary>
    /// Adres e-mail użytkownika.
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UserPermissionsUpdatedEvent"/>.
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika.</param>
    /// <param name="externalId">Identyfikator zewnętrzny użytkownika.</param>
    /// <param name="email">Adres e-mail użytkownika.</param>
    public UserPermissionsUpdatedEvent(Guid userId, string externalId, string email)
    {
        UserId = userId;
        ExternalId = externalId;
        Email = email;
    }
}
