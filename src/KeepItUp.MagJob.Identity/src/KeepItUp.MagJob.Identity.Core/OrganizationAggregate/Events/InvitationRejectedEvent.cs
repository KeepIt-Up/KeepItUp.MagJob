
namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

/// <summary>
/// Zdarzenie informujące o odrzuceniu zaproszenia do organizacji.
/// </summary>
public class InvitationRejectedEvent : DomainEventBase
{
  /// <summary>
  /// Identyfikator organizacji.
  /// </summary>
  public Guid OrganizationId { get; }

  /// <summary>
  /// Identyfikator zaproszenia.
  /// </summary>
  public Guid InvitationId { get; }

  /// <summary>
  /// Tworzy nowe zdarzenie informujące o odrzuceniu zaproszenia do organizacji.
  /// </summary>
  /// <param name="organizationId">Identyfikator organizacji.</param>
  /// <param name="invitationId">Identyfikator zaproszenia.</param>
  public InvitationRejectedEvent(Guid organizationId, Guid invitationId)
  {
    OrganizationId = organizationId;
    InvitationId = invitationId;
  }
}