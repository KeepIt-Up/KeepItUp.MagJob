
namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

/// <summary>
/// Domain event informing about the creation of a new role in an organization.
/// </summary>
public class RoleCreatedEvent : DomainEventBase
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
    /// Creates a new RoleCreatedEvent.
    /// </summary>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="roleId">Role ID.</param>
    /// <param name="name">Role name.</param>
    public RoleCreatedEvent(Guid organizationId, Guid roleId, string name)
    {
        OrganizationId = organizationId;
        RoleId = roleId;
        Name = name;
    }
}
