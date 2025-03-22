
namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

/// <summary>
/// Zdarzenie informujące o akceptacji zaproszenia do organizacji.
/// </summary>
public class InvitationAcceptedEvent : DomainEventBase
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
  /// Identyfikator użytkownika, który zaakceptował zaproszenie.
  /// </summary>
  public Guid UserId { get; }

  /// <summary>
  /// Tworzy nowe zdarzenie informujące o akceptacji zaproszenia do organizacji.
  /// </summary>
  /// <param name="organizationId">Identyfikator organizacji.</param>
  /// <param name="invitationId">Identyfikator zaproszenia.</param>
  /// <param name="userId">Identyfikator użytkownika, który zaakceptował zaproszenie.</param>
  public InvitationAcceptedEvent(Guid organizationId, Guid invitationId, Guid userId)
  {
    OrganizationId = organizationId;
    InvitationId = invitationId;
    UserId = userId;
  }
}