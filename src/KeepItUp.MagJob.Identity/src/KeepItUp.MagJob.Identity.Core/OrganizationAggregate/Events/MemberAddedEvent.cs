
namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

/// <summary>
/// Domain event informing about the addition of a new member to an organization.
/// </summary>
public class MemberAddedEvent : DomainEventBase
{
    /// <summary>
    /// Organization ID.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// User ID.
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Role ID.
    /// </summary>
    public Guid RoleId { get; }

    /// <summary>
    /// Creates a new MemberAddedEvent.
    /// </summary>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="userId">User ID.</param>
    /// <param name="roleId">Role ID.</param>
    public MemberAddedEvent(Guid organizationId, Guid userId, Guid roleId)
    {
        OrganizationId = organizationId;
        UserId = userId;
        RoleId = roleId;
    }
}