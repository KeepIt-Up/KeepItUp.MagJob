namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

/// <summary>
/// Domain event informing about the update of an organization's banner.
/// </summary>
public class OrganizationBannerUpdatedEvent : DomainEventBase
{
    /// <summary>
    /// Organization ID.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// URL of the organization's banner.
    /// </summary>
    public string? BannerUrl { get; }

    /// <summary>
    /// Creates a new OrganizationBannerUpdatedEvent.
    /// </summary>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="bannerUrl">URL of the organization's banner.</param>
    public OrganizationBannerUpdatedEvent(Guid organizationId, string? bannerUrl)
    {
        OrganizationId = organizationId;
        BannerUrl = bannerUrl;
    }
}
