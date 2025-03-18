namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

/// <summary>
/// Zdarzenie aktualizacji bannera organizacji.
/// </summary>
public class OrganizationBannerUpdatedEvent : DomainEventBase
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// URL do bannera organizacji.
    /// </summary>
    public string? BannerUrl { get; }

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="OrganizationBannerUpdatedEvent"/>.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="bannerUrl">URL do bannera organizacji.</param>
    public OrganizationBannerUpdatedEvent(Guid organizationId, string? bannerUrl)
    {
        OrganizationId = organizationId;
        BannerUrl = bannerUrl;
    }
}
