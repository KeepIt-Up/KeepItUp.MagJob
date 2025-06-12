
namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

/// <summary>
/// Domain event informing about the deletion of a role from an organization.
/// </summary>
public class RoleDeletedEvent : DomainEventBase
{
    /// <summary>
    /// Organization ID.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Role ID.
    /// </summary>
    public Guid RoleId { get; }

    /// <summary>
    /// Role name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Creates a new RoleDeletedEvent.
    /// </summary>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="roleId">Role ID.</param>
    /// <param name="name">Role name.</param>
    public RoleDeletedEvent(Guid organizationId, Guid roleId, string name)
    {
        OrganizationId = organizationId;
        RoleId = roleId;
        Name = name;
    }
}