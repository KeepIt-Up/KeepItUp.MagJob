using KeepItUp.MagJob.Identity.Core.InvitationAggregate;

namespace KeepItUp.MagJob.Identity.UnitTests.Core.InvitationAggregate;

/// <summary>
/// Builder pattern implementation for Invitation aggregate.
/// Provides fluent API for creating Invitation instances in tests.
/// </summary>
public class InvitationBuilder
{
    private Guid _organizationId = Guid.NewGuid();
    private string _email = "invited@example.com";
    private Guid _roleId = Guid.NewGuid();
    private DateTime _expiresAt = DateTime.UtcNow.AddDays(7);
    private bool _shouldAccept = false;
    private bool _shouldReject = false;
    private bool _shouldExpire = false;

    /// <summary>
    /// Sets the organization ID.
    /// </summary>
    public InvitationBuilder ForOrganization(Guid organizationId)
    {
        _organizationId = organizationId;
        return this;
    }

    /// <summary>
    /// Sets the email address.
    /// </summary>
    public InvitationBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    /// <summary>
    /// Sets the role ID.
    /// </summary>
    public InvitationBuilder WithRole(Guid roleId)
    {
        _roleId = roleId;
        return this;
    }

    /// <summary>
    /// Sets the expiration date.
    /// </summary>
    public InvitationBuilder WithExpiration(DateTime expiresAt)
    {
        _expiresAt = expiresAt;
        return this;
    }

    /// <summary>
    /// Sets the invitation to expire in the specified number of days.
    /// </summary>
    public InvitationBuilder ExpiringInDays(int days)
    {
        _expiresAt = DateTime.UtcNow.AddDays(days);
        return this;
    }

    /// <summary>
    /// Sets the invitation to expire in the specified number of hours.
    /// </summary>
    public InvitationBuilder ExpiringInHours(int hours)
    {
        _expiresAt = DateTime.UtcNow.AddHours(hours);
        return this;
    }

    /// <summary>
    /// Sets the invitation to expire in the specified number of minutes.
    /// </summary>
    public InvitationBuilder ExpiringInMinutes(int minutes)
    {
        _expiresAt = DateTime.UtcNow.AddMinutes(minutes);
        return this;
    }

    /// <summary>
    /// Creates an already expired invitation.
    /// </summary>
    public InvitationBuilder Expired()
    {
        _expiresAt = DateTime.UtcNow.AddDays(-1);
        return this;
    }

    /// <summary>
    /// Creates an invitation that expires soon.
    /// </summary>
    public InvitationBuilder ExpiringSoon()
    {
        _expiresAt = DateTime.UtcNow.AddMinutes(5);
        return this;
    }

    /// <summary>
    /// Creates a long-lived invitation.
    /// </summary>
    public InvitationBuilder LongLived()
    {
        _expiresAt = DateTime.UtcNow.AddYears(1);
        return this;
    }

    /// <summary>
    /// Configures the invitation to be accepted after creation.
    /// </summary>
    public InvitationBuilder Accepted()
    {
        _shouldAccept = true;
        _shouldReject = false;
        _shouldExpire = false;
        return this;
    }

    /// <summary>
    /// Configures the invitation to be rejected after creation.
    /// </summary>
    public InvitationBuilder Rejected()
    {
        _shouldAccept = false;
        _shouldReject = true;
        _shouldExpire = false;
        return this;
    }

    /// <summary>
    /// Configures the invitation to be manually expired after creation.
    /// </summary>
    public InvitationBuilder ManuallyExpired()
    {
        _shouldAccept = false;
        _shouldReject = false;
        _shouldExpire = true;
        return this;
    }

    /// <summary>
    /// Creates an invitation for edge case testing.
    /// </summary>
    public InvitationBuilder ForEdgeCases()
    {
        _email = "a@b.co"; // Short but valid email
        _expiresAt = DateTime.UtcNow.AddMinutes(1); // Expires very soon
        return this;
    }

    /// <summary>
    /// Builds the Invitation instance.
    /// </summary>
    public Invitation Build()
    {
        var invitation = Invitation.Create(_organizationId, _email, _roleId, _expiresAt);

        // Apply post-creation actions
        if (_shouldAccept)
        {
            invitation.Accept();
        }
        else if (_shouldReject)
        {
            invitation.Reject();
        }
        else if (_shouldExpire)
        {
            invitation.MarkAsExpired();
        }

        return invitation;
    }

    /// <summary>
    /// Creates a new InvitationBuilder with default values.
    /// </summary>
    public static InvitationBuilder New() => new InvitationBuilder();

    /// <summary>
    /// Creates a new InvitationBuilder with valid default values.
    /// </summary>
    public static InvitationBuilder Valid() => new InvitationBuilder();

    /// <summary>
    /// Creates multiple invitations using the current builder configuration.
    /// Each invitation will have a unique email address.
    /// </summary>
    public List<Invitation> BuildMany(int count)
    {
        var invitations = new List<Invitation>();
        for (int i = 0; i < count; i++)
        {
            var builder = new InvitationBuilder
            {
                _organizationId = _organizationId,
                _email = $"invited{i + 1}@example.com",
                _roleId = _roleId,
                _expiresAt = _expiresAt,
                _shouldAccept = _shouldAccept,
                _shouldReject = _shouldReject,
                _shouldExpire = _shouldExpire
            };
            invitations.Add(builder.Build());
        }
        return invitations;
    }

    /// <summary>
    /// Creates invitations with different statuses for testing.
    /// Returns a list containing Pending, Accepted, Rejected, and Expired invitations.
    /// </summary>
    public static List<Invitation> BuildWithDifferentStatuses(Guid organizationId, Guid roleId)
    {
        return new List<Invitation>
        {
            New().ForOrganization(organizationId).WithRole(roleId).WithEmail("pending@example.com").Build(),
            New().ForOrganization(organizationId).WithRole(roleId).WithEmail("accepted@example.com").Accepted().Build(),
            New().ForOrganization(organizationId).WithRole(roleId).WithEmail("rejected@example.com").Rejected().Build(),
            New().ForOrganization(organizationId).WithRole(roleId).WithEmail("expired@example.com").ManuallyExpired().Build()
        };
    }
}