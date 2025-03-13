
namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

/// <summary>
/// Zdarzenie informujące o utworzeniu nowego zaproszenia do organizacji.
/// </summary>
public class InvitationCreatedEvent : DomainEventBase
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
  /// Adres e-mail osoby zapraszanej.
  /// </summary>
  public string Email { get; }

  /// <summary>
  /// Tworzy nowe zdarzenie informujące o utworzeniu zaproszenia do organizacji.
  /// </summary>
  /// <param name="organizationId">Identyfikator organizacji.</param>
  /// <param name="invitationId">Identyfikator zaproszenia.</param>
  /// <param name="email">Adres e-mail osoby zapraszanej.</param>
  public InvitationCreatedEvent(Guid organizationId, Guid invitationId, string email)
  {
    OrganizationId = organizationId;
    InvitationId = invitationId;
    Email = email;
  }
}