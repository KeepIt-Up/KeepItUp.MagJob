
namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

/// <summary>
/// Domain event informing about the creation of a new organization.
/// </summary>
public class OrganizationCreatedEvent : DomainEventBase
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
    /// Creates a new OrganizationCreatedEvent.
    /// </summary>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="name">Organization name.</param>
    /// <param name="ownerId">Owner ID.</param>
    public OrganizationCreatedEvent(Guid organizationId, string name, Guid ownerId)
    {
        OrganizationId = organizationId;
        Name = name;
        OwnerId = ownerId;
    }
}