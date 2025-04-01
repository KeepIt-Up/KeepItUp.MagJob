
namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

/// <summary>
/// Zdarzenie informujące o usunięciu członka z organizacji.
/// </summary>
public class MemberRemovedEvent : DomainEventBase
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
    /// Tworzy nowe zdarzenie informujące o usunięciu członka z organizacji.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="userId">Identyfikator użytkownika.</param>
    public MemberRemovedEvent(Guid organizationId, Guid userId)
    {
        OrganizationId = organizationId;
        UserId = userId;
    }
}