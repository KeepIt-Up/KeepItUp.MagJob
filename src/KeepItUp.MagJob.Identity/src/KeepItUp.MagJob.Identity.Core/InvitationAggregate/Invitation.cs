using KeepItUp.MagJob.Identity.Core.InvitationAggregate.Events;

namespace KeepItUp.MagJob.Identity.Core.InvitationAggregate;

/// <summary>
/// Represents an invitation to an organization - Aggregate Root.
/// </summary>
public class Invitation : BaseEntity, IAggregateRoot
{
    /// <summary>
    /// Organization ID.
    /// </summary>
    public Guid OrganizationId { get; private set; }

    /// <summary>
    /// Email address of the invited user.
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// Invitation token.
    /// </summary>
    public string Token { get; private set; } = string.Empty;

    /// <summary>
    /// ID of the role that will be assigned after accepting the invitation.
    /// </summary>
    public Guid RoleId { get; private set; }

    /// <summary>
    /// Status of the invitation.
    /// </summary>
    public InvitationStatus Status { get; private set; } = InvitationStatus.Pending;

    /// <summary>
    /// Expiration date of the invitation.
    /// </summary>
    public DateTime ExpiresAt { get; private set; }

    /// <summary>
    /// Whether the invitation has expired.
    /// </summary>
    public bool IsExpired => Status == InvitationStatus.Expired || DateTime.UtcNow > ExpiresAt;

    /// <summary>
    /// Private constructor for EF Core and factory creation.
    /// </summary>
    private Invitation() { }

    /// <summary>
    /// Creates a new invitation to an organization.
    /// </summary>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="email">Email address of the invited user.</param>
    /// <param name="roleId">ID of the role that will be assigned after accepting the invitation.</param>
    /// <param name="expiresAt">Expiration date of the invitation.</param>
    /// <returns>New invitation.</returns>
    public static Invitation Create(Guid organizationId, string email, Guid roleId, DateTime? expiresAt = null)
    {
        Guard.Against.Default(organizationId, nameof(organizationId));
        Guard.Against.NullOrEmpty(email, nameof(email));
        Guard.Against.Default(roleId, nameof(roleId));

        var invitation = new Invitation
        {
            OrganizationId = organizationId,
            Email = email,
            Token = Guid.NewGuid().ToString(),
            RoleId = roleId,
            ExpiresAt = expiresAt ?? DateTime.UtcNow.AddDays(7)
        };

        invitation.RegisterDomainEventAndUpdate(new InvitationCreatedEvent(invitation.Id, organizationId, email, roleId));

        return invitation;
    }

    /// <summary>
    /// Accepts the invitation.
    /// </summary>
    public void Accept()
    {
        if (Status != InvitationStatus.Pending)
        {
            throw new InvalidOperationException("Tylko oczekujące zaproszenia mogą zostać zaakceptowane.");
        }

        if (IsExpired)
        {
            throw new InvalidOperationException("Nie można zaakceptować wygasłego zaproszenia.");
        }

        Status = InvitationStatus.Accepted;

        RegisterDomainEventAndUpdate(new InvitationAcceptedEvent(Id, OrganizationId, Email, RoleId));
    }

    /// <summary>
    /// Rejects the invitation.
    /// </summary>
    public void Reject()
    {
        if (Status != InvitationStatus.Pending)
        {
            throw new InvalidOperationException("Tylko oczekujące zaproszenia mogą zostać odrzucone.");
        }

        if (IsExpired)
        {
            throw new InvalidOperationException("Nie można odrzucić wygasłego zaproszenia.");
        }

        Status = InvitationStatus.Rejected;

        RegisterDomainEventAndUpdate(new InvitationRejectedEvent(Id, OrganizationId, Email));
    }

    /// <summary>
    /// Marks the invitation as expired.
    /// </summary>
    public void MarkAsExpired()
    {
        if (Status != InvitationStatus.Pending)
        {
            return;
        }

        Status = InvitationStatus.Expired;

        RegisterDomainEventAndUpdate(new InvitationExpiredEvent(Id, OrganizationId, Email));
    }
}