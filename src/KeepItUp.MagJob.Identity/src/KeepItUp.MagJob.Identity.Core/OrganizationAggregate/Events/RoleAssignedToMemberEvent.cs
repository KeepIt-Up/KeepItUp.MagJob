namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

/// <summary>
/// Zdarzenie informujące o przypisaniu roli do członka organizacji.
/// </summary>
public class RoleAssignedToMemberEvent : DomainEventBase
{
    /// <summary>
    /// Identyfikator członka.
    /// </summary>
    public Guid MemberId { get; }

    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Identyfikator przypisanej roli.
    /// </summary>
    public Guid RoleId { get; }

    /// <summary>
    /// Tworzy nowe zdarzenie informujące o przypisaniu roli do członka organizacji.
    /// </summary>
    /// <param name="memberId">Identyfikator członka.</param>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="userId">Identyfikator użytkownika.</param>
    /// <param name="roleId">Identyfikator przypisanej roli.</param>
    public RoleAssignedToMemberEvent(Guid memberId, Guid organizationId, Guid userId, Guid roleId)
    {
        MemberId = memberId;
        OrganizationId = organizationId;
        UserId = userId;
        RoleId = roleId;
    }
}
