
namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

/// <summary>
/// Zdarzenie informujące o dezaktywacji organizacji.
/// </summary>
public class OrganizationDeactivatedEvent : DomainEventBase
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
  /// Tworzy nowe zdarzenie informujące o dezaktywacji organizacji.
  /// </summary>
  /// <param name="organizationId">Identyfikator organizacji.</param>
  /// <param name="name">Nazwa organizacji.</param>
  /// <param name="ownerId">Identyfikator właściciela organizacji.</param>
  public OrganizationDeactivatedEvent(Guid organizationId, string name, Guid ownerId)
  {
    OrganizationId = organizationId;
    Name = name;
    OwnerId = ownerId;
  }
}