namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

/// <summary>
/// Zdarzenie informujące o aktualizacji uprawnień roli w organizacji.
/// </summary>
public class RolePermissionsUpdatedEvent : DomainEventBase
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Identyfikator roli.
    /// </summary>
    public Guid RoleId { get; }

    /// <summary>
    /// Nazwa roli.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Tworzy nowe zdarzenie informujące o aktualizacji uprawnień roli w organizacji.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="roleId">Identyfikator roli.</param>
    /// <param name="name">Nazwa roli.</param>
    public RolePermissionsUpdatedEvent(Guid organizationId, Guid roleId, string name)
    {
        OrganizationId = organizationId;
        RoleId = roleId;
        Name = name;
    }
}
