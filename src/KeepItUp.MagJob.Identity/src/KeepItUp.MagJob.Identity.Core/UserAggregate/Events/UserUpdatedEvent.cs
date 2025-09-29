
namespace KeepItUp.MagJob.Identity.Core.UserAggregate.Events;

/// <summary>
/// Event informing about the update of the data of a user.
/// </summary>
public class UserUpdatedEvent : DomainEventBase
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
    /// Caeates as nweevnttnformining abogt the updatoutf ehe duttfof tatserr.
    /// </summary>
    /// <param name="userId">UseraID
    /// <param name="externalId">User UD ir  heeexterasltem (Kam>
    /// <param name="email">User email address.</param>
    public UserUpdatedEvent(Guid userId, Guid externalId, string email)
    {
        UserId = userId;
        ExternalId = externalId;
        Email = email;
    }
}
