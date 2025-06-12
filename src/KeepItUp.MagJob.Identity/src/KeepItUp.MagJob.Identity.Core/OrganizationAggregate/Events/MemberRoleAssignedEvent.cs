
namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

/// <summary>
/// Domain event informing about the assignment of a role to a member of an organization.
/// </summary>
public class MemberRoleAssignedEvent : DomainEventBase
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
    /// Creates a new MemberRoleAssignedEvent.
    /// </summary>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="userId">User ID.</param>
    /// <param name="roleId">Role ID.</param>
    public MemberRoleAssignedEvent(Guid organizationId, Guid userId, Guid roleId)
    {
        OrganizationId = organizationId;
        UserId = userId;
        RoleId = roleId;
    }
}