using KeepItUp.MagJob.Identity.Core.InvitationAggregate;

namespace KeepItUp.MagJob.Identity.UnitTests.Core.InvitationAggregate;

/// <summary>
/// Object Mother pattern implementation for Invitation aggregate.
/// Provides pre-configured Invitation instances for testing.
/// </summary>
public static class InvitationMother
{
    /// <summary>
    /// Creates a basic valid invitation with default values.
    /// </summary>
    public static Invitation ValidInvitation() => Invitation.Create(
        organizationId: Guid.NewGuid(),
        email: "invited@example.com",
        roleId: Guid.NewGuid(),
        expiresAt: DateTime.UtcNow.AddDays(7));

    /// <summary>
    /// Creates an invitation with custom email.
    /// </summary>
    public static Invitation InvitationWithEmail(string email) => Invitation.Create(
        organizationId: Guid.NewGuid(),
        email: email,
        roleId: Guid.NewGuid(),
        expiresAt: DateTime.UtcNow.AddDays(7));

    /// <summary>
    /// Creates an invitation with custom organization ID.
    /// </summary>
    public static Invitation InvitationForOrganization(Guid organizationId) => Invitation.Create(
        organizationId: organizationId,
        email: "invited@example.com",
        roleId: Guid.NewGuid(),
        expiresAt: DateTime.UtcNow.AddDays(7));

    /// <summary>
    /// Creates an invitation with custom role ID.
    /// </summary>
    public static Invitation InvitationWithRole(Guid roleId) => Invitation.Create(
        organizationId: Guid.NewGuid(),
        email: "invited@example.com",
        roleId: roleId,
        expiresAt: DateTime.UtcNow.AddDays(7));

    /// <summary>
    /// Creates an invitation with custom expiration date.
    /// </summary>
    public static Invitation InvitationWithExpiration(DateTime expiresAt) => Invitation.Create(
        organizationId: Guid.NewGuid(),
        email: "invited@example.com",
        roleId: Guid.NewGuid(),
        expiresAt: expiresAt);

    /// <summary>
    /// Creates an expired invitation.
    /// </summary>
    public static Invitation ExpiredInvitation() => Invitation.Create(
        organizationId: Guid.NewGuid(),
        email: "expired@example.com",
        roleId: Guid.NewGuid(),
        expiresAt: DateTime.UtcNow.AddDays(-1)); // Expired yesterday

    /// <summary>
    /// Creates an invitation that expires soon (in 1 hour).
    /// </summary>
    public static Invitation InvitationExpiringSoon() => Invitation.Create(
        organizationId: Guid.NewGuid(),
        email: "expiring@example.com",
        roleId: Guid.NewGuid(),
        expiresAt: DateTime.UtcNow.AddHours(1));

    /// <summary>
    /// Creates an accepted invitation.
    /// </summary>
    public static Invitation AcceptedInvitation()
    {
        var invitation = ValidInvitation();
        invitation.Accept();
        return invitation;
    }

    /// <summary>
    /// Creates a rejected invitation.
    /// </summary>
    public static Invitation RejectedInvitation()
    {
        var invitation = ValidInvitation();
        invitation.Reject();
        return invitation;
    }

    /// <summary>
    /// Creates a manually expired invitation.
    /// </summary>
    public static Invitation ManuallyExpiredInvitation()
    {
        var invitation = ValidInvitation();
        invitation.MarkAsExpired();
        return invitation;
    }

    /// <summary>
    /// Creates multiple invitations for batch testing.
    /// </summary>
    public static List<Invitation> MultipleInvitations(int count = 3)
    {
        var invitations = new List<Invitation>();
        var organizationId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        for (int i = 0; i < count; i++)
        {
            invitations.Add(Invitation.Create(
                organizationId: organizationId,
                email: $"invited{i + 1}@example.com",
                roleId: roleId,
                expiresAt: DateTime.UtcNow.AddDays(7)));
        }
        return invitations;
    }

    /// <summary>
    /// Creates invitations with different statuses for testing.
    /// </summary>
    public static List<Invitation> InvitationsWithDifferentStatuses()
    {
        return new List<Invitation>
        {
            ValidInvitation(),      // Pending
            AcceptedInvitation(),   // Accepted
            RejectedInvitation(),   // Rejected
            ManuallyExpiredInvitation() // Expired
        };
    }

    /// <summary>
    /// Creates an invitation for edge case testing.
    /// </summary>
    public static Invitation InvitationForEdgeCases() => Invitation.Create(
        organizationId: Guid.NewGuid(),
        email: "a@b.co", // Short but valid email
        roleId: Guid.NewGuid(),
        expiresAt: DateTime.UtcNow.AddMinutes(1)); // Expires very soon

    /// <summary>
    /// Creates an invitation with long expiration (1 year).
    /// </summary>
    public static Invitation LongLivedInvitation() => Invitation.Create(
        organizationId: Guid.NewGuid(),
        email: "longlived@example.com",
        roleId: Guid.NewGuid(),
        expiresAt: DateTime.UtcNow.AddYears(1));
}