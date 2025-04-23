
namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

/// <summary>
/// Zdarzenie aktualizacji logo organizacji.
/// </summary>
public class OrganizationLogoUpdatedEvent : DomainEventBase
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// URL do logo organizacji.
    /// </summary>
    public string? LogoUrl { get; }

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="OrganizationLogoUpdatedEvent"/>.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="logoUrl">URL do logo organizacji.</param>
    public OrganizationLogoUpdatedEvent(Guid organizationId, string? logoUrl)
    {
        OrganizationId = organizationId;
        LogoUrl = logoUrl;
    }
}
