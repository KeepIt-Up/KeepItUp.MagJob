namespace KeepItUp.MagJob.Identity.Core.InvitationAggregate.Events;

/// <summary>
/// Domain event informing about the creation of a new invitation to an organization.
/// </summary>
public class InvitationCreatedEvent : DomainEventBase
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
    /// Role ID that will be assigned after the invitation is accepted.
    /// </summary>
    public Guid RoleId { get; }

    /// <summary>
    /// Creates a new InvitationCreatedEvent.
    /// </summary>
    /// <param name="invitationId">Invitation ID.</param>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="email">Email address of the invited person.</param>
    /// <param name="roleId">Role ID that will be assigned after the invitation is accepted.</param>
    public InvitationCreatedEvent(Guid invitationId, Guid organizationId, string email, Guid roleId)
    {
        InvitationId = invitationId;
        OrganizationId = organizationId;
        Email = email;
        RoleId = roleId;
    }
}