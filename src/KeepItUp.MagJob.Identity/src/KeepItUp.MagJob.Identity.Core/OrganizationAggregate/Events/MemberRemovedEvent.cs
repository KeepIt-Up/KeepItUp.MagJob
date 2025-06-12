
namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

/// <summary>
/// Domain event informing about the removal of a member from an organization.
/// </summary>
public class MemberRemovedEvent : DomainEventBase
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
    /// Creates a new MemberRemovedEvent.
    /// </summary>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="userId">User ID.</param>
    public MemberRemovedEvent(Guid organizationId, Guid userId)
    {
        OrganizationId = organizationId;
        UserId = userId;
    }
}