namespace KeepItUp.MagJob.Identity.Core.UserAggregate.Events;

/// <summary>
/// Event informing about the update of the last login date of a user.
/// </summary>
public class UserLastLoginUpdatedEvent : DomainEventBase
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
    /// Last login date of the user.
    /// </summary>
    public DateTime LastLoginDate { get; }

    /// <summary>
    /// Creates a new event informing about the update of the last login date of a user.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="externalId">User ID in the external system (Keycloak).</param>
    /// <param name="email">User email address.</param>
    /// <param name="lastLoginDate">Last login date of the user.</param>
    public UserLastLoginUpdatedEvent(Guid userId, Guid externalId, string email, DateTime lastLoginDate)
    {
        UserId = userId;
        ExternalId = externalId;
        Email = email;
        LastLoginDate = lastLoginDate;
    }
}
