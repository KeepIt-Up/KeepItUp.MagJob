namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

/// <summary>
/// Zdarzenie informujące o utworzeniu nowego członka organizacji.
/// </summary>
public class MemberCreatedEvent : DomainEventBase
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
    /// Identyfikator początkowej roli.
    /// </summary>
    public Guid RoleId { get; }

    /// <summary>
    /// Tworzy nowe zdarzenie informujące o utworzeniu członka organizacji.
    /// </summary>
    /// <param name="memberId">Identyfikator członka.</param>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="userId">Identyfikator użytkownika.</param>
    /// <param name="roleId">Identyfikator początkowej roli.</param>
    public MemberCreatedEvent(Guid memberId, Guid organizationId, Guid userId, Guid roleId)
    {
        MemberId = memberId;
        OrganizationId = organizationId;
        UserId = userId;
        RoleId = roleId;
    }
}
