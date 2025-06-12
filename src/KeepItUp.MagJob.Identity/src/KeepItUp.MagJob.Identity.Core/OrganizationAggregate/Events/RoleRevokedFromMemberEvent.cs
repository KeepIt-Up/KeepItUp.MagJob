namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

/// <summary>
/// Domain event informing about the revocation of a role from a member of an organization.
/// </summary>
public class RoleRevokedFromMemberEvent : DomainEventBase
{
    /// <summary>
    /// Member ID.
    /// </summary>
    public Guid MemberId { get; }

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
    /// Creates a new RoleRevokedFromMemberEvent.
    /// </summary>
    /// <param name="memberId">Member ID.</param>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="userId">User ID.</param>
    /// <param name="roleId">Role ID.</param>
    public RoleRevokedFromMemberEvent(Guid memberId, Guid organizationId, Guid userId, Guid roleId)
    {
        MemberId = memberId;
        OrganizationId = organizationId;
        UserId = userId;
        RoleId = roleId;
    }
}
