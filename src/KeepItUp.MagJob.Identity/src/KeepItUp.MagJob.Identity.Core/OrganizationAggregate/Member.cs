using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate;

/// <summary>
/// Represents a member of an organization.
/// </summary>
public class Member : BaseEntity
{
    /// <summary>
    /// User ID.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Organization ID.
    /// </summary>
    public Guid OrganizationId { get; private set; }

    /// <summary>
    /// List of roles assigned to the member (navigation property for EF Core).
    /// </summary>
    public virtual ICollection<Role> Roles { get; private set; }

    /// <summary>
    /// Date of joining the organization.
    /// </summary>
    public DateTime JoinedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Private constructor for EF Core and factory creation.
    /// </summary>
    private Member()
    {
        Roles = new HashSet<Role>();
    }

    /// <summary>
    /// Creates a new member of an organization.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="initialRole">Initial role for the member.</param>
    /// <returns>New member of an organization.</returns>
    public static Member Create(Guid userId, Guid organizationId, Role initialRole)
    {
        Guard.Against.Default(userId, nameof(userId));
        Guard.Against.Default(organizationId, nameof(organizationId));
        Guard.Against.Null(initialRole, nameof(initialRole));

        var member = new Member
        {
            UserId = userId,
            OrganizationId = organizationId,
            Roles = new HashSet<Role>()
        };

        member.Roles.Add(initialRole);
        member.RegisterDomainEventAndUpdate(new MemberCreatedEvent(member.Id, organizationId, userId, initialRole.Id));

        return member;
    }

    /// <summary>
    /// Assigns a new role to a member of an organization.
    /// </summary>
    /// <param name="role">Role to assign.</param>
    public void AssignRole(Role role)
    {
        Guard.Against.Null(role, nameof(role));

        if (!Roles.Any(r => r.Id == role.Id))
        {
            Roles.Add(role);
            RegisterDomainEventAndUpdate(new RoleAssignedToMemberEvent(Id, OrganizationId, UserId, role.Id));
        }
    }

    /// <summary>
    /// Removes a role assigned to a member of an organization.
    /// </summary>
    /// <param name="role">Role to remove.</param>
    /// <returns>True, if the role was removed; otherwise false.</returns>
    public bool RemoveRole(Role role)
    {
        Guard.Against.Null(role, nameof(role));

        // Don't allow removing the last role
        if (Roles.Count <= 1)
        {
            return false;
        }

        bool removed = Roles.Remove(role);

        if (removed)
        {
            RegisterDomainEventAndUpdate(new RoleRevokedFromMemberEvent(Id, OrganizationId, UserId, role.Id));
        }

        return removed;
    }

    /// <summary>
    /// Checks if a member has a specific role.
    /// </summary>
    /// <param name="roleId">ID of the role.</param>
    /// <returns>True, if the member has the role; otherwise false.</returns>
    public bool HasRole(Guid roleId)
    {
        return Roles.Any(r => r.Id == roleId);
    }

    /// <summary>
    /// Checks if a member has a specific role.
    /// </summary>
    /// <param name="role">Role to check.</param>
    /// <returns>True, if the member has the role; otherwise false.</returns>
    public bool HasRole(Role role)
    {
        Guard.Against.Null(role, nameof(role));
        return Roles.Contains(role);
    }

    /// <summary>
    /// Gets all role IDs for this member.
    /// </summary>
    public IEnumerable<Guid> GetRoleIds()
    {
        return Roles.Select(r => r.Id);
    }
}
