
namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

/// <summary>
/// Zdarzenie informujące o przypisaniu roli do członka organizacji.
/// </summary>
public class MemberRoleAssignedEvent : DomainEventBase
{
  /// <summary>
  /// Identyfikator organizacji.
  /// </summary>
  public Guid OrganizationId { get; }

  /// <summary>
  /// Identyfikator użytkownika.
  /// </summary>
  public Guid UserId { get; }

  /// <summary>
  /// Identyfikator roli.
  /// </summary>
  public Guid RoleId { get; }

  /// <summary>
  /// Tworzy nowe zdarzenie informujące o przypisaniu roli do członka organizacji.
  /// </summary>
  /// <param name="organizationId">Identyfikator organizacji.</param>
  /// <param name="userId">Identyfikator użytkownika.</param>
  /// <param name="roleId">Identyfikator roli.</param>
  public MemberRoleAssignedEvent(Guid organizationId, Guid userId, Guid roleId)
  {
    OrganizationId = organizationId;
    UserId = userId;
    RoleId = roleId;
  }
}