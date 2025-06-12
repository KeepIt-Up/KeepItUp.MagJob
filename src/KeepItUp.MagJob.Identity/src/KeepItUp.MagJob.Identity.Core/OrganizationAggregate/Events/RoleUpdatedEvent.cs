namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

/// <summary>
/// Domain event informing about the update of a role in an organization.
/// </summary>
public class RoleUpdatedEvent : DomainEventBase
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
    /// Creates a new RoleUpdatedEvent.
    /// </summary>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="roleId">Role ID.</param>
    /// <param name="name">Role name.</param>
    public RoleUpdatedEvent(Guid organizationId, Guid roleId, string name)
    {
        OrganizationId = organizationId;
        RoleId = roleId;
        Name = name;
    }
}
