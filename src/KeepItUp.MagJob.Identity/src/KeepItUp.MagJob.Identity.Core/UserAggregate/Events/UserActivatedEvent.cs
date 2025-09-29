
namespace KeepItUp.MagJob.Identity.Core.UserAggregate.Events;

/// <summary>
/// Event informing about the activation of a user.
/// </summary>
public class UserActivatedEvent : DomainEventBase
{
    /// <summary>
    /// User ID.
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// User ID in the external system (Keycloak).
    /// </summary>
    public Guid ExternalId { get; }

    /// <summary>
    /// User email address.
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Creates a new event informing about the activation of a user.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="externalId">User ID in the external system (Keycloak).</param>
    /// <param name="email">User email address.</param>
    public UserActivatedEvent(Guid userId, Guid externalId, string email)
    {
        UserId = userId;
        ExternalId = externalId;
        Email = email;
    }
}
