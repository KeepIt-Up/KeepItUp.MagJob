
namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

/// <summary>
/// Domain event informing about the activation of an organization.
/// </summary>
public class OrganizationActivatedEvent : DomainEventBase
{
    /// <summary>
    /// Organization ID.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Organization name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Owner ID.
    /// </summary>
    public Guid OwnerId { get; }

    /// <summary>
    /// Creates a new OrganizationActivatedEvent.
    /// </summary>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="name">Organization name.</param>
    /// <param name="ownerId">Owner ID.</param>
    public OrganizationActivatedEvent(Guid organizationId, string name, Guid ownerId)
    {
        OrganizationId = organizationId;
        Name = name;
        OwnerId = ownerId;
    }
}