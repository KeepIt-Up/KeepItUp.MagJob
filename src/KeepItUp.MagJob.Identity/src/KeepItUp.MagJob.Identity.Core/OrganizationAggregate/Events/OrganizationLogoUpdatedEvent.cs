
namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

/// <summary>
/// Domain event informing about the update of an organization's logo.
/// </summary>
public class OrganizationLogoUpdatedEvent : DomainEventBase
{
    /// <summary>
    /// Organization ID.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// URL of the organization's logo.
    /// </summary>
    public string? LogoUrl { get; }

    /// <summary>
    /// Creates a new OrganizationLogoUpdatedEvent.
    /// </summary>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="logoUrl">URL of the organization's logo.</param>
    public OrganizationLogoUpdatedEvent(Guid organizationId, string? logoUrl)
    {
        OrganizationId = organizationId;
        LogoUrl = logoUrl;
    }
}
