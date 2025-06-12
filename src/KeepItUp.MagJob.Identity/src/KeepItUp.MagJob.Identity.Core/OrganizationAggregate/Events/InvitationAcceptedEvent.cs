namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

/// <summary>
/// Domain event informing about the acceptance of an invitation to an organization.
/// </summary>
public class InvitationAcceptedEvent : DomainEventBase
{
    /// <summary>
    /// Invitation ID.
    /// </summary>
    public Guid InvitationId { get; }

    /// <summary>
    /// Organization ID.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Email address of the invited person.
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Role ID that will be assigned.
    /// </summary>
    public Guid RoleId { get; }

    /// <summary>
    /// Creates a new InvitationAcceptedEvent.
    /// </summary>
    /// <param name="invitationId">Invitation ID.</param>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="email">Email address of the invited person.</param>
    /// <param name="roleId">Role ID that will be assigned.</param>
    public InvitationAcceptedEvent(Guid invitationId, Guid organizationId, string email, Guid roleId)
    {
        InvitationId = invitationId;
        OrganizationId = organizationId;
        Email = email;
        RoleId = roleId;
    }
}
