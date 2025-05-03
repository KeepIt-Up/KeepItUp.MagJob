
namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

/// <summary>
/// Zdarzenie informujące o utworzeniu nowej organizacji.
/// </summary>
public class OrganizationCreatedEvent : DomainEventBase
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Nazwa organizacji.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Identyfikator właściciela organizacji.
    /// </summary>
    public Guid OwnerId { get; }

    /// <summary>
    /// Tworzy nowe zdarzenie informujące o utworzeniu organizacji.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="name">Nazwa organizacji.</param>
    /// <param name="ownerId">Identyfikator właściciela organizacji.</param>
    public OrganizationCreatedEvent(Guid organizationId, string name, Guid ownerId)
    {
        OrganizationId = organizationId;
        Name = name;
        OwnerId = ownerId;
    }
}