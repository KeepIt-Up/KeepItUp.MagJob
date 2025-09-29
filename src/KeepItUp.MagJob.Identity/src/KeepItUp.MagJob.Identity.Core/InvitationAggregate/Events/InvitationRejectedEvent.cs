namespace KeepItUp.MagJob.Identity.Core.InvitationAggregate.Events;

/// <summary>
/// Domain event informing about the rejection of an invitation to an organization.
/// </summary>
public class InvitationRejectedEvent : DomainEventBase
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
    /// Creates a new InvitationRejectedEvent.
    /// </summary>
    /// <param name="invitationId">Invitation ID.</param>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="email">Email address of the invited person.</param>
    public InvitationRejectedEvent(Guid invitationId, Guid organizationId, string email)
    {
        InvitationId = invitationId;
        OrganizationId = organizationId;
        Email = email;
    }
}